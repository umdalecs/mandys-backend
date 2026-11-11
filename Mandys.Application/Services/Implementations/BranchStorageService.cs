using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;

namespace Mandys.Application.Services.Implementations;

public class BranchStorageService(
    IBranchStorageRepository branchStorages,
    IBranchRepository branches,
    IProductRepository products) : IBranchStorageService
{
    public async Task<PagedBranchStorageResponse> GetBranchStorageAsync(
        int branchId, int page, int pageSize, string? search, string? orderBy, bool? isSupply)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await branchStorages.SearchByBranchAsync(
            branchId, search, page, pageSize, orderBy, isSupply);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedBranchStorageResponse(
            totalCount,
            page,
            pageSize,
            totalPages,
            items.Select(bs => bs.ToResponse()).ToList());
    }

    public async Task<BranchStorageResponse> GetBranchStorageByProductAsync(int branchId, int productId)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        var storage = await branchStorages.GetByBranchAndProductAsync(branchId, productId);
        if (storage is null)
        {
            throw ServiceException.NotFound(
                $"No se encontró el registro de almacenamiento para la sucursal '{branchId}' y el producto '{productId}'.");
        }

        return storage.ToResponse();
    }

    public async Task<BranchStorageResponse> UpdateMinimumStockAsync(
        int branchId, int productId, UpdateBranchStorageRequest request)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        var storage = await branchStorages.GetByBranchAndProductAsync(branchId, productId);
        if (storage is null)
        {
            throw ServiceException.NotFound(
                $"No se encontró el registro de almacenamiento para la sucursal '{branchId}' y el producto '{productId}'.");
        }

        if (!MeasureUnits.IsValidQuantity(storage.Product.MeasureUnit, request.MinimumStock))
        {
            throw ServiceException.BadRequest(
                $"El producto '{storage.Product.Description}' se mide en '{storage.Product.MeasureUnit}' y no permite cantidades decimales.");
        }

        storage.UpdateMinimumStock(request.MinimumStock);

        await branchStorages.UpdateAsync(storage);

        return storage.ToResponse();
    }

    public async Task<BranchStorageHistoryResponse> CreateInMovementAsync(
        int branchId, CreateStorageInRequest request, string? userRole = null)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        var normalizedFolio = request.Folio.Trim();
        var normalizedSupplierName = request.SupplierName.Trim();
        if (await branchStorages.ExistsHistoryByFolioAsync(branchId, normalizedFolio))
        {
            throw ServiceException.Conflict(
                $"Ya existe un movimiento con el folio '{normalizedFolio}' en esta sucursal.");
        }

        if (request.Products is null || request.Products.Count == 0)
        {
            throw ServiceException.BadRequest("El movimiento debe incluir al menos un producto.");
        }

        var isCentralWarehouseChief = string.Equals(
            userRole, Roles.CentralWarehouseChief, StringComparison.OrdinalIgnoreCase);
        var isBranchWarehouseChief = string.Equals(
            userRole, Roles.WarehouseChief, StringComparison.OrdinalIgnoreCase);

        var requiresCostPrice = isCentralWarehouseChief || (!isBranchWarehouseChief && branch.WarehouseOnly);
        var applyCostPrice = requiresCostPrice;

        if (requiresCostPrice && request.Products.Any(p => !p.CostPrice.HasValue))
        {
            throw ServiceException.BadRequest(
                "El costo de compra es obligatorio para todos los productos en entradas de almacén central.");
        }

        var productIds = request.Products.Select(l => l.ProductId).Distinct().ToList();
        var storages = await branchStorages.GetByBranchAndProductsAsync(branchId, productIds);
        var storageByProductId = storages.ToDictionary(s => s.Product.Id);

        var historyLines = new List<BranchStorageHistoryProduct>();
        var updatedStorages = new List<BranchStorage>();

        foreach (var line in request.Products)
        {
            if (!storageByProductId.TryGetValue(line.ProductId, out var storage))
            {
                var product = await products.GetByIdAsync(line.ProductId);
                if (product is null)
                {
                    throw ServiceException.NotFound($"No se encontró el producto con ID '{line.ProductId}'.");
                }

                throw ServiceException.NotFound(
                    $"No se encontró el registro de almacenamiento para el producto '{product.Description}' (ID '{line.ProductId}') en la sucursal '{branchId}'.");
            }

            if (!MeasureUnits.IsValidQuantity(storage.Product.MeasureUnit, line.Quantity))
            {
                throw ServiceException.BadRequest(
                    $"El producto '{storage.Product.Description}' se mide en '{storage.Product.MeasureUnit}' y no permite cantidades decimales.");
            }

            storage.AddStock(line.Quantity);

            decimal recordedCostPrice = 0;
            if (applyCostPrice && line.CostPrice.HasValue)
            {
                storage.Product.UpdateCostPrice(line.CostPrice.Value);
                recordedCostPrice = line.CostPrice.Value;
            }
            else if (!applyCostPrice)
            {
                recordedCostPrice = storage.Product.CostPrice ?? 0;
            }

            updatedStorages.Add(storage);
            historyLines.Add(new BranchStorageHistoryProduct(storage.Product, line.Quantity, recordedCostPrice));
        }

        var history = new BranchStorageHistory(
            0,
            branch,
            StorageMovementTypes.In,
            normalizedFolio,
            normalizedSupplierName,
            null,
            historyLines);

        var created = await branchStorages.ApplyMovementAsync(history, updatedStorages);

        return created.ToResponse();
    }

    public async Task<BranchStorageHistoryResponse> CreateOutMovementAsync(
        int branchId, CreateStorageOutRequest request)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        var normalizedReason = request.Reason.Trim();

        if (request.Products is null || request.Products.Count == 0)
        {
            throw ServiceException.BadRequest("El movimiento debe incluir al menos un producto.");
        }

        var productIds = request.Products.Select(l => l.ProductId).Distinct().ToList();
        var storages = await branchStorages.GetByBranchAndProductsAsync(branchId, productIds);
        var storageByProductId = storages.ToDictionary(s => s.Product.Id);

        var historyLines = new List<BranchStorageHistoryProduct>();
        var updatedStorages = new List<BranchStorage>();

        foreach (var line in request.Products)
        {
            if (!storageByProductId.TryGetValue(line.ProductId, out var storage))
            {
                var product = await products.GetByIdAsync(line.ProductId);
                if (product is null)
                {
                    throw ServiceException.NotFound($"No se encontró el producto con ID '{line.ProductId}'.");
                }

                throw ServiceException.NotFound(
                    $"No se encontró el registro de almacenamiento para el producto '{product.Description}' (ID '{line.ProductId}') en la sucursal '{branchId}'.");
            }

            if (!MeasureUnits.IsValidQuantity(storage.Product.MeasureUnit, line.Quantity))
            {
                throw ServiceException.BadRequest(
                    $"El producto '{storage.Product.Description}' se mide en '{storage.Product.MeasureUnit}' y no permite cantidades decimales.");
            }

            if (storage.CurrentStock < line.Quantity)
            {
                throw ServiceException.BadRequest(
                    $"Stock insuficiente para el producto '{storage.Product.Description}'. Stock actual: {storage.CurrentStock}, solicitado: {line.Quantity}.");
            }

            storage.RemoveStock(line.Quantity);

            updatedStorages.Add(storage);
            historyLines.Add(new BranchStorageHistoryProduct(storage.Product, line.Quantity, storage.Product.CostPrice ?? 0));
        }

        var history = new BranchStorageHistory(
            0,
            branch,
            StorageMovementTypes.Out,
            null,
            null,
            normalizedReason,
            historyLines);

        var created = await branchStorages.ApplyMovementAsync(history, updatedStorages);

        return created.ToResponse();
    }

    public async Task<PagedBranchStorageHistoryResponse> GetBranchStorageHistoryAsync(
        int branchId, int page, int pageSize, string? search, string? type, string? orderBy)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        if (!string.IsNullOrWhiteSpace(type) && !StorageMovementTypes.IsValid(type))
        {
            throw ServiceException.BadRequest(
                $"El tipo de movimiento '{type}' no es válido. Valores permitidos: {string.Join(", ", StorageMovementTypes.All)}.");
        }

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await branchStorages.SearchHistoryByBranchAsync(
            branchId, search, type, page, pageSize, orderBy);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedBranchStorageHistoryResponse(
            totalCount,
            page,
            pageSize,
            totalPages,
            items.Select(h => h.ToResponse()).ToList());
    }

    public async Task<BranchStorageHistoryResponse> GetBranchStorageHistoryByIdAsync(
        int branchId, int historyId)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        var history = await branchStorages.GetHistoryByIdAsync(branchId, historyId);
        if (history is null)
        {
            throw ServiceException.NotFound(
                $"No se encontró el movimiento de almacenamiento con ID '{historyId}' en la sucursal '{branchId}'.");
        }

        return history.ToResponse();
    }
}
