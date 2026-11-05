using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;

namespace Mandys.Application.Services.Implementations;

public class BranchStorageService(
    IBranchStorageRepository branchStorages,
    IBranchRepository branches) : IBranchStorageService
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
}
