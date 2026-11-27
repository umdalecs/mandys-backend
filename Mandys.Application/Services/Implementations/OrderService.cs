using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;

namespace Mandys.Application.Services.Implementations;

public class OrderService(
    IOrderRepository orders,
    IBranchRepository branches,
    IUserRepository users,
    IProductRepository products,
    IDishRepository dishes,
    IComboRepository combos,
    IBranchStorageRepository branchStorages) : IOrderService
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

        var order = new Order(
            0,
            branch,
            user,
            paymentMethod,
            cardLast4,
            orderProducts,
            orderDishes,
            orderCombos);

        var created = await orders.CreateWithStockDeductionAsync(order, updatedStorages, historyLines);
        return created.ToResponse();
    }

    public async Task<PagedOrdersResponse> GetOrdersAsync(
        int? branchId,
        int userId,
        string userRole,
        int? userBranchId,
        string? paymentMethod,
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

        var (scopedBranchId, scopedUserId) = ResolveQueryScope(branchId, userId, userRole, userBranchId);

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await orders.SearchAsync(
            scopedBranchId,
            scopedUserId,
            paymentMethod,
            page,
            pageSize,
            orderBy);

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

        return order.ToResponse();
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
            if (order.CreatedByUser.ID != userId)
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
}
