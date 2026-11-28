using Mandys.Application.Configuration;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Microsoft.Extensions.Options;

namespace Mandys.Application.Services.Implementations;

public class OrderService(
    IOrderRepository orders,
    IBranchRepository branches,
    IUserRepository users,
    IProductRepository products,
    IDishRepository dishes,
    IComboRepository combos,
    IBranchStorageRepository branchStorages,
    IDrawerReconciliationRepository drawerReconciliations,
    IKitchenOrderNotifier kitchenOrderNotifier,
    IOptions<LoyaltyOptions> loyaltyOptions) : IOrderService
{
    public async Task<OrderResponse> CreateOrderAsync(
        int branchId,
        int userId,
        string userRole,
        int? userBranchId,
        CreateOrderRequest request)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        if (branch.WarehouseOnly)
        {
            throw ServiceException.BadRequest(
                $"La sucursal '{branch.Name}' es solo de almacén y no puede recibir pedidos.");
        }

        var user = await users.GetByIdAsync(userId);
        if (user is null)
        {
            throw ServiceException.NotFound($"No se encontró el usuario con ID '{userId}'.");
        }

        var (paymentMethod, cardLast4) = ResolvePaymentMethod(branchId, userRole, userBranchId, request);
        var customer = await ResolveOrderCustomerAsync(user, userRole, request.CustomerId);

        var orderProducts = await ResolveProductLinesAsync(request.Products);
        var orderDishes = await ResolveDishLinesAsync(request.Dishes);
        var orderCombos = await ResolveComboLinesAsync(request.Combos);

        if (orderProducts.Count == 0 && orderDishes.Count == 0 && orderCombos.Count == 0)
        {
            throw ServiceException.BadRequest(
                "El pedido debe incluir al menos un producto, platillo o combo.");
        }

        var requiredByProductId = AggregateRequiredStock(orderProducts, orderDishes, orderCombos);

        var updatedStorages = new List<BranchStorage>();
        var historyLines = new List<BranchStorageHistoryProduct>();

        if (requiredByProductId.Count > 0)
        {
            var storages = await branchStorages.GetByBranchAndProductsAsync(
                branchId,
                requiredByProductId.Keys.ToList());
            var storageByProductId = storages.ToDictionary(s => s.Product.Id);

            foreach (var (productId, (product, requiredQty)) in requiredByProductId)
            {
                if (!storageByProductId.TryGetValue(productId, out var storage))
                {
                    throw ServiceException.BadRequest(
                        $"No hay existencias de '{product.Description}' en el almacén de la sucursal '{branch.Name}'.");
                }

                if (storage.CurrentStock < requiredQty)
                {
                    throw ServiceException.BadRequest(
                        $"Stock insuficiente para '{product.Description}' en la sucursal '{branch.Name}'. Disponible: {storage.CurrentStock}, requerido: {requiredQty}.");
                }

                storage.RemoveStock(requiredQty);
                updatedStorages.Add(storage);
                historyLines.Add(new BranchStorageHistoryProduct(
                    storage.Product,
                    requiredQty,
                    storage.Product.CostPrice ?? 0m));
            }
        }

        var pointsPercentage = loyaltyOptions.Value.PointsPercentage;
        if (pointsPercentage < 0)
        {
            throw ServiceException.BadRequest(
                "El porcentaje de puntos configurado no puede ser negativo.");
        }

        var pointsToSpend = Math.Round(request.PointsToSpend ?? 0m, 2, MidpointRounding.AwayFromZero);
        if (pointsToSpend < 0)
        {
            throw ServiceException.BadRequest("Los puntos a utilizar no pueden ser negativos.");
        }

        if (pointsToSpend > 0)
        {
            if (customer is null)
            {
                throw ServiceException.BadRequest(
                    "Debe seleccionar un cliente para utilizar puntos en el pedido.");
            }

            if (pointsToSpend > customer.Points)
            {
                throw ServiceException.BadRequest(
                    $"Puntos insuficientes. El cliente tiene {customer.Points} puntos disponibles y se intentaron usar {pointsToSpend}.");
            }
        }

        var subtotalAmount =
            orderProducts.Sum(p => p.Quantity * p.UnitPrice) +
            orderDishes.Sum(d => d.Quantity * d.UnitPrice) +
            orderCombos.Sum(c => c.Quantity * c.UnitPrice);

        if (pointsToSpend > subtotalAmount)
        {
            throw ServiceException.BadRequest(
                $"Los puntos a utilizar ({pointsToSpend}) no pueden superar el subtotal del pedido ({subtotalAmount}).");
        }

        var totalAmount = subtotalAmount - pointsToSpend;

        decimal? amountPaid = null;
        decimal? changeAmount = null;
        if (paymentMethod == PaymentMethods.Cash)
        {
            if (!request.AmountPaid.HasValue)
            {
                if (totalAmount == 0m)
                {
                    amountPaid = 0m;
                }
                else
                {
                    throw ServiceException.BadRequest(
                        "El monto pagado en efectivo (amountPaid) es obligatorio para pedidos en efectivo.");
                }
            }
            else
            {
                amountPaid = Math.Round(request.AmountPaid.Value, 2, MidpointRounding.AwayFromZero);
            }

            if (amountPaid.Value < totalAmount)
            {
                throw ServiceException.BadRequest(
                    $"El monto pagado ({amountPaid.Value}) no puede ser menor al total del pedido ({totalAmount}).");
            }

            changeAmount = Math.Round(amountPaid.Value - totalAmount, 2, MidpointRounding.AwayFromZero);
        }

        var pointsEarned = customer is not null
            ? Order.CalculatePoints(totalAmount, pointsPercentage)
            : 0m;

        var order = new Order(
            0,
            branch,
            user,
            paymentMethod,
            cardLast4,
            orderProducts,
            orderDishes,
            orderCombos,
            customer: customer,
            pointsEarned: pointsEarned,
            pointsSpent: pointsToSpend,
            amountPaid: amountPaid,
            changeAmount: changeAmount);

        var created = await orders.CreateWithStockDeductionAsync(order, updatedStorages, historyLines);
        var response = created.ToResponse();
        await kitchenOrderNotifier.NotifyOrderCreatedAsync(response);
        return response;
    }

    public async Task<PagedOrdersResponse> GetOrdersAsync(
        int? branchId,
        int userId,
        string userRole,
        int? userBranchId,
        string? paymentMethod,
        string? status,
        int page,
        int pageSize,
        string? orderBy)
    {
        if (branchId.HasValue)
        {
            var branch = await branches.GetByIdAsync(branchId.Value);
            if (branch is null)
            {
                throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId.Value}'.");
            }
        }

        if (!string.IsNullOrWhiteSpace(paymentMethod) && !PaymentMethods.IsValid(paymentMethod))
        {
            throw ServiceException.BadRequest(
                $"El método de pago '{paymentMethod}' no es válido. Valores permitidos: {string.Join(", ", PaymentMethods.All)}.");
        }

        if (!string.IsNullOrWhiteSpace(status) && !OrderStatuses.IsValid(status))
        {
            throw ServiceException.BadRequest(
                $"El estatus '{status}' no es válido. Valores permitidos: {string.Join(", ", OrderStatuses.All)}.");
        }

        var (scopedBranchId, scopedUserId) = ResolveQueryScope(branchId, userId, userRole, userBranchId);

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        DateTime? createdAtMax = null;
        if (string.Equals(userRole, Roles.BranchChief, StringComparison.OrdinalIgnoreCase) &&
            scopedBranchId.HasValue)
        {
            createdAtMax = await drawerReconciliations.GetLatestReconciliationAtByBranchAsync(scopedBranchId.Value);
            if (!createdAtMax.HasValue)
            {
                return new PagedOrdersResponse(0, page, pageSize, 0, []);
            }
        }

        var (totalCount, items) = await orders.SearchAsync(
            scopedBranchId,
            scopedUserId,
            paymentMethod,
            status,
            page,
            pageSize,
            orderBy,
            createdAtMax);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedOrdersResponse(
            totalCount,
            page,
            pageSize,
            totalPages,
            items.Select(o => o.ToResponse()).ToList());
    }

    public async Task<OrderResponse> GetOrderByIdAsync(
        int? branchId,
        int orderId,
        int userId,
        string userRole,
        int? userBranchId)
    {
        var order = branchId.HasValue
            ? await orders.GetByBranchAndIdAsync(branchId.Value, orderId)
            : await orders.GetByIdAsync(orderId);

        if (order is null)
        {
            throw ServiceException.NotFound($"No se encontró el pedido con ID '{orderId}'.");
        }

        EnsureOrderReadAccess(order, userId, userRole, userBranchId);

        if (string.Equals(userRole, Roles.BranchChief, StringComparison.OrdinalIgnoreCase))
        {
            var lastReconciliationAt = await drawerReconciliations
                .GetLatestReconciliationAtByBranchAsync(order.Branch.Id);

            if (!lastReconciliationAt.HasValue || order.CreatedAt > lastReconciliationAt.Value)
            {
                throw ServiceException.NotFound($"No se encontró el pedido con ID '{orderId}'.");
            }
        }

        return order.ToResponse();
    }

    public async Task<OrderResponse> MarkOrderCompletedAsync(
        int orderId,
        string userRole,
        int? userBranchId)
    {
        var order = await LoadKitchenChiefOrderAsync(orderId, userRole, userBranchId);

        if (order.Status == OrderStatuses.Ready)
        {
            throw ServiceException.Conflict("El pedido ya se encuentra en estado 'preparado'.");
        }

        order.MarkAsCompleted();
        await orders.UpdateAsync(order);

        var response = order.ToResponse();
        await kitchenOrderNotifier.NotifyOrderCompletedAsync(response);
        return response;
    }

    public async Task<OrderResponse> MarkOrderUncompletedAsync(
        int orderId,
        string userRole,
        int? userBranchId)
    {
        var order = await LoadKitchenChiefOrderAsync(orderId, userRole, userBranchId);

        if (order.Status != OrderStatuses.Ready)
        {
            throw ServiceException.Conflict("El pedido ya se encuentra en estado 'en preparación'.");
        }

        var now = DateTime.UtcNow;
        if (!order.CompletedAt.HasValue ||
            now - order.CompletedAt.Value > TimeSpan.FromMinutes(Order.UncompleteWindowMinutes))
        {
            throw ServiceException.Conflict(
                $"Solo se puede revertir un pedido durante los {Order.UncompleteWindowMinutes} minutos posteriores a marcarlo como preparado.");
        }

        order.MarkAsUncompleted(now);
        await orders.UpdateAsync(order);

        var response = order.ToResponse();
        await kitchenOrderNotifier.NotifyOrderUncompletedAsync(response);
        return response;
    }

    private async Task<Order> LoadKitchenChiefOrderAsync(int orderId, string userRole, int? userBranchId)
    {
        if (!string.Equals(userRole, Roles.KitchenChief, StringComparison.OrdinalIgnoreCase))
        {
            throw ServiceException.BadRequest("Solo el jefe de cocina puede cambiar el estatus de un pedido.");
        }

        if (!userBranchId.HasValue)
        {
            throw ServiceException.BadRequest("El usuario actual no tiene una sucursal asignada.");
        }

        var order = await orders.GetByIdAsync(orderId);
        if (order is null || order.Branch.Id != userBranchId.Value)
        {
            throw ServiceException.NotFound($"No se encontró el pedido con ID '{orderId}'.");
        }

        return order;
    }

    private static (string PaymentMethod, string? CardLast4) ResolvePaymentMethod(
        int branchId,
        string userRole,
        int? userBranchId,
        CreateOrderRequest request)
    {
        if (string.Equals(userRole, Roles.Customer, StringComparison.OrdinalIgnoreCase))
        {
            if (request.CardPayment is null)
            {
                throw ServiceException.BadRequest(
                    "Los pedidos de clientes requieren pago con tarjeta bancaria (cardPayment).");
            }

            if (request.AmountPaid.HasValue)
            {
                throw ServiceException.BadRequest(
                    "Los pedidos pagados con tarjeta no deben incluir monto pagado en efectivo (amountPaid).");
            }

            var digits = new string(request.CardPayment.CardNumber.Where(char.IsAsciiDigit).ToArray());
            if (digits.Length < 13 || digits.Length > 19)
            {
                throw ServiceException.BadRequest("El número de tarjeta bancaria no es válido.");
            }

            // Simulated bank card authorization: in-memory mock approval.
            var last4 = digits[^4..];
            return (PaymentMethods.Card, last4);
        }

        if (string.Equals(userRole, Roles.Cashier, StringComparison.OrdinalIgnoreCase))
        {
            if (!userBranchId.HasValue || userBranchId.Value != branchId)
            {
                throw ServiceException.BadRequest(
                    "Un cajero solo puede registrar pedidos en su propia sucursal asignada.");
            }

            if (request.CardPayment is not null)
            {
                throw ServiceException.BadRequest(
                    "Los pedidos registrados por cajero solo aceptan pago en efectivo y no deben incluir datos de tarjeta.");
            }

            return (PaymentMethods.Cash, null);
        }

        throw ServiceException.BadRequest("Solo los clientes y cajeros pueden crear pedidos.");
    }

    private async Task<List<OrderProduct>> ResolveProductLinesAsync(
        IReadOnlyList<CreateOrderProductLineRequest>? lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return [];
        }

        var result = new List<OrderProduct>();
        foreach (var line in lines)
        {
            var product = await products.GetByIdAsync(line.ProductId);
            if (product is null)
            {
                throw ServiceException.NotFound($"No se encontró el producto con ID '{line.ProductId}'.");
            }

            if (product.IsSupply)
            {
                throw ServiceException.BadRequest(
                    $"El producto '{product.Description}' (ID '{product.Id}') es un insumo y no puede venderse directamente en un pedido.");
            }

            result.Add(new OrderProduct(product, line.Quantity, product.SalePrice ?? 0m));
        }

        return result;
    }

    private async Task<List<OrderDish>> ResolveDishLinesAsync(
        IReadOnlyList<CreateOrderDishLineRequest>? lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return [];
        }

        var result = new List<OrderDish>();
        foreach (var line in lines)
        {
            var dish = await dishes.GetByIdAsync(line.DishId);
            if (dish is null)
            {
                throw ServiceException.NotFound($"No se encontró el platillo con ID '{line.DishId}'.");
            }

            result.Add(new OrderDish(dish, line.Quantity, dish.Price));
        }

        return result;
    }

    private async Task<List<OrderCombo>> ResolveComboLinesAsync(
        IReadOnlyList<CreateOrderComboLineRequest>? lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return [];
        }

        var result = new List<OrderCombo>();
        foreach (var line in lines)
        {
            var combo = await combos.GetByIdAsync(line.ComboId);
            if (combo is null)
            {
                throw ServiceException.NotFound($"No se encontró el combo con ID '{line.ComboId}'.");
            }

            result.Add(new OrderCombo(combo, line.Quantity, combo.Price));
        }

        return result;
    }

    private static Dictionary<int, (Product Product, decimal RequiredQuantity)> AggregateRequiredStock(
        IEnumerable<OrderProduct> orderProducts,
        IEnumerable<OrderDish> orderDishes,
        IEnumerable<OrderCombo> orderCombos)
    {
        var required = new Dictionary<int, (Product Product, decimal RequiredQuantity)>();

        void AddRequirement(Product product, decimal qty)
        {
            if (required.TryGetValue(product.Id, out var existing))
            {
                required[product.Id] = (existing.Product, existing.RequiredQuantity + qty);
            }
            else
            {
                required[product.Id] = (product, qty);
            }
        }

        foreach (var line in orderProducts)
        {
            AddRequirement(line.Product, line.Quantity);
        }

        foreach (var line in orderDishes)
        {
            foreach (var ingredient in line.Dish.Recipe)
            {
                AddRequirement(ingredient.Product, ingredient.Quantity * line.Quantity);
            }
        }

        foreach (var line in orderCombos)
        {
            foreach (var comboProduct in line.Combo.Products)
            {
                AddRequirement(comboProduct.Product, comboProduct.Quantity * line.Quantity);
            }

            foreach (var comboDish in line.Combo.Dishes)
            {
                foreach (var ingredient in comboDish.Dish.Recipe)
                {
                    AddRequirement(ingredient.Product, ingredient.Quantity * comboDish.Quantity * line.Quantity);
                }
            }
        }

        return required;
    }

    private static (int? BranchId, int? CreatedByUserId) ResolveQueryScope(
        int? requestedBranchId,
        int userId,
        string userRole,
        int? userBranchId)
    {
        if (string.Equals(userRole, Roles.Administrator, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(userRole, Roles.OpChief, StringComparison.OrdinalIgnoreCase))
        {
            return (requestedBranchId, null);
        }

        if (string.Equals(userRole, Roles.Customer, StringComparison.OrdinalIgnoreCase))
        {
            return (requestedBranchId, userId);
        }

        if (!userBranchId.HasValue)
        {
            throw ServiceException.BadRequest("El usuario actual no tiene una sucursal asignada.");
        }

        if (requestedBranchId.HasValue && requestedBranchId.Value != userBranchId.Value)
        {
            throw ServiceException.BadRequest("Solo puedes consultar pedidos de tu propia sucursal.");
        }

        return (userBranchId.Value, null);
    }

    private static void EnsureOrderReadAccess(
        Order order,
        int userId,
        string userRole,
        int? userBranchId)
    {
        if (string.Equals(userRole, Roles.Administrator, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(userRole, Roles.OpChief, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (string.Equals(userRole, Roles.Customer, StringComparison.OrdinalIgnoreCase))
        {
            if (order.CreatedByUser.ID != userId && order.Customer?.ID != userId)
            {
                throw ServiceException.NotFound($"No se encontró el pedido con ID '{order.Id}'.");
            }

            return;
        }

        if (!userBranchId.HasValue || order.Branch.Id != userBranchId.Value)
        {
            throw ServiceException.NotFound($"No se encontró el pedido con ID '{order.Id}'.");
        }
    }

    private async Task<User?> ResolveOrderCustomerAsync(User currentUser, string userRole, int? requestedCustomerId)
    {
        if (string.Equals(userRole, Roles.Customer, StringComparison.OrdinalIgnoreCase))
        {
            if (requestedCustomerId.HasValue && requestedCustomerId.Value != currentUser.ID)
            {
                throw ServiceException.BadRequest("Un cliente no puede asignar un pedido a otro cliente.");
            }

            return currentUser;
        }

        if (string.Equals(userRole, Roles.Cashier, StringComparison.OrdinalIgnoreCase))
        {
            if (!requestedCustomerId.HasValue)
            {
                return null;
            }

            var customer = await users.GetByIdAsync(requestedCustomerId.Value);
            if (customer is null)
            {
                throw ServiceException.NotFound($"No se encontró el cliente con ID '{requestedCustomerId.Value}'.");
            }

            if (!string.Equals(customer.Role, Roles.Customer, StringComparison.OrdinalIgnoreCase))
            {
                throw ServiceException.BadRequest(
                    $"El usuario con ID '{requestedCustomerId.Value}' no tiene rol de cliente.");
            }

            return customer;
        }

        return null;
    }
}
