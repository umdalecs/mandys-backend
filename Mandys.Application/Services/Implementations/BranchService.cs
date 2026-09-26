using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class BranchService(IBranchRepository branches) : IBranchService
{
    public async Task<PagedBranchesResponse> GetBranchesAsync(int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await branches.SearchAsync(search, page, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedBranchesResponse(totalCount, page, pageSize, totalPages,
            items.Select(b => b.ToResponse()).ToList());
    }

    public async Task<BranchResponse> GetBranchByIdAsync(int id)
    {
        var branch = await branches.GetByIdAsync(id);
        if (branch is null)
        {
            throw ServiceException.NotFound($"Branch with ID '{id}' not found.");
        }

        return branch.ToResponse();
    }

    public async Task<BranchResponse> CreateBranchAsync(CreateBranchRequest request)
    {
        var created = await branches.AddAsync(new Branch(
            0,
            request.Name.Trim(),
            request.Address.Trim(),
            request.WarehouseOnly));

        return created.ToResponse();
    }

    public async Task<BranchResponse> UpdateBranchAsync(int id, UpdateBranchRequest request)
    {
        var branch = await branches.GetByIdAsync(id);
        if (branch is null)
        {
            throw ServiceException.NotFound($"Branch with ID '{id}' not found.");
        }

        branch.UpdateDetails(
            string.IsNullOrWhiteSpace(request.Name) ? branch.Name : request.Name.Trim(),
            string.IsNullOrWhiteSpace(request.Address) ? branch.Address : request.Address.Trim(),
            request.WarehouseOnly ?? branch.WarehouseOnly);

        await branches.UpdateAsync(branch);

        return branch.ToResponse();
    }

    public async Task DeleteBranchAsync(int id)
    {
        // Staff keep the branch their role is scoped to, and the FK is set to
        // null on delete, so refuse while users are still assigned.
        var userCount = await branches.CountUsersAsync(id);
        if (userCount > 0)
        {
            throw ServiceException.Conflict(
                $"Branch with ID '{id}' still has {userCount} user(s) assigned. Reassign or remove them first.");
        }

        var removed = await branches.RemoveAsync(id);
        if (!removed)
        {
            throw ServiceException.NotFound($"Branch with ID '{id}' not found.");
        }
    }
}
