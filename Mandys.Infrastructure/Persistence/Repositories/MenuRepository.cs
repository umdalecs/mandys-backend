using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class MenuRepository(ApplicationDbContext db) : IMenuRepository
{
    public async Task<(int TotalCount, IReadOnlyList<Product> Items)> SearchProductsAsync(
        string? search, int page, int pageSize, string? orderBy)
    {
        IQueryable<ProductRecord> query = db.Products
            .AsNoTracking()
            .Where(p => !p.IsSupply);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p => p.Description.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .ApplyOrdering(
                OrderByParser.Parse<MenuSortField>(orderBy)
                    .Select(k => (k.Field.ProductSelector(), k.Descending))
                    .ToList(),
                p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task<(int TotalCount, IReadOnlyList<Dish> Items)> SearchDishesAsync(
        string? search, int page, int pageSize, string? orderBy)
    {
        // Recipes are intentionally not included: the menu doesn't expose them.
        IQueryable<DishRecord> query = db.Dishes
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(d => d.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .ApplyOrdering(
                OrderByParser.Parse<MenuSortField>(orderBy)
                    .Select(k => (k.Field.DishSelector(), k.Descending))
                    .ToList(),
                d => d.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task<(int TotalCount, IReadOnlyList<Combo> Items)> SearchCombosAsync(
        string? search, int page, int pageSize, string? orderBy)
    {
        IQueryable<ComboRecord> query = db.Combos
            .AsNoTracking()
            .Include(c => c.ComboDishes.OrderBy(l => l.Id))
            .ThenInclude(l => l.Dish)
            .Include(c => c.ComboProducts.OrderBy(l => l.Id))
            .ThenInclude(l => l.Product);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .ApplyOrdering(
                OrderByParser.Parse<MenuSortField>(orderBy)
                    .Select(k => (k.Field.ComboSelector(), k.Descending))
                    .ToList(),
                c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }
}
