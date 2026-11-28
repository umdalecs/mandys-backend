using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class DrawerTransactionRepository(ApplicationDbContext db) : IDrawerTransactionRepository
{
    public async Task<(decimal CurrentBalance, int TransactionCount, DateTime? LastTransactionAt)> GetBalanceAsync(
        int branchId,
        int userId)
    {
        var summary = await db.DrawerTransactions
            .AsNoTracking()
            .Where(dt => dt.BranchId == branchId && dt.UserId == userId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                CurrentBalance = g.Sum(dt => dt.Amount),
                TransactionCount = g.Count(),
                LastTransactionAt = g.Max(dt => (DateTime?)dt.CreatedAt),
            })
            .FirstOrDefaultAsync();

        if (summary is null)
        {
            return (0m, 0, null);
        }

        return (
            Math.Round(summary.CurrentBalance, 2, MidpointRounding.AwayFromZero),
            summary.TransactionCount,
            summary.LastTransactionAt);
    }

    public async Task<IReadOnlyList<DrawerBalanceSummary>> GetBranchBalancesAsync(int branchId)
    {
        var rows = await db.DrawerTransactions
            .AsNoTracking()
            .Where(dt => dt.BranchId == branchId)
            .GroupBy(dt => new
            {
                dt.BranchId,
                BranchName = dt.Branch.Name,
                dt.UserId,
                dt.User.FirstName,
                dt.User.FatherLastName,
                dt.User.MotherLastName,
            })
            .Select(g => new
            {
                g.Key.BranchId,
                g.Key.BranchName,
                g.Key.UserId,
                g.Key.FirstName,
                g.Key.FatherLastName,
                g.Key.MotherLastName,
                CurrentBalance = g.Sum(dt => dt.Amount),
                TransactionCount = g.Count(),
                LastTransactionAt = g.Max(dt => (DateTime?)dt.CreatedAt),
            })
            .OrderBy(r => r.UserId)
            .ToListAsync();

        return rows
            .Select(r => new DrawerBalanceSummary(
                r.BranchId,
                r.BranchName,
                r.UserId,
                FormatFullName(r.FirstName, r.FatherLastName, r.MotherLastName),
                Math.Round(r.CurrentBalance, 2, MidpointRounding.AwayFromZero),
                r.TransactionCount,
                r.LastTransactionAt))
            .ToList();
    }

    public async Task<(int TotalCount, IReadOnlyList<DrawerTransaction> Items)> SearchAsync(
        int? branchId,
        int? userId,
        string? type,
        int page,
        int pageSize,
        string? orderBy)
    {
        var query = db.DrawerTransactions
            .AsNoTracking()
            .Include(dt => dt.Branch)
            .Include(dt => dt.User)
            .AsQueryable();

        if (branchId.HasValue)
        {
            query = query.Where(dt => dt.BranchId == branchId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(dt => dt.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(type) && DrawerTransactionTypes.IsValid(type))
        {
            var normalizedType = DrawerTransactionTypes.Normalize(type);
            query = query.Where(dt => dt.Type == normalizedType);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .ApplyOrdering(
                OrderByParser.Parse<DrawerTransactionSortField>(orderBy)
                    .Select(k => (k.Field.Selector(), k.Descending))
                    .ToList(),
                dt => dt.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    private static string FormatFullName(string firstName, string fatherLastName, string? motherLastName) =>
        string.IsNullOrWhiteSpace(motherLastName)
            ? $"{firstName} {fatherLastName}"
            : $"{firstName} {fatherLastName} {motherLastName}";
}
