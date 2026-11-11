using System.Linq.Expressions;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Whitelisted sortable fields for the catalog list endpoints. Only these
/// may appear in <c>?orderBy=</c>; anything else is dropped by the parser,
/// so no raw client string ever reaches LINQ.
/// </summary>
internal enum ProductSortField
{
    Id,
    Description,
    IsSupply,
    CostPrice,
    SalePrice,
    MeasureUnit,
    CreatedAt,
    UpdatedAt,
}

/// <inheritdoc cref="ProductSortField"/>
internal enum DishSortField
{
    Id,
    Name,
    Price,
    CreatedAt,
    UpdatedAt,
}

/// <inheritdoc cref="ProductSortField"/>
internal enum ComboSortField
{
    Id,
    Name,
    Price,
    CreatedAt,
    UpdatedAt,
}

/// <inheritdoc cref="ProductSortField"/>
internal enum BranchSortField
{
    Id,
    Name,
    Address,
    WarehouseOnly,
    CreatedAt,
    UpdatedAt,
}

/// <inheritdoc cref="ProductSortField"/>
internal enum UserSortField
{
    Id,
    FirstName,
    FatherLastName,
    MotherLastName,
    Email,
    CreatedAt,
    UpdatedAt,
}

/// <inheritdoc cref="ProductSortField"/>
internal enum BranchStorageSortField
{
    Id,
    ProductId,
    Description,
    MinimumStock,
    CurrentStock,
    LastUpdated,
    CreatedAt,
    UpdatedAt,
}

/// <inheritdoc cref="ProductSortField"/>
internal enum BranchStorageHistorySortField
{
    Id,
    Type,
    Folio,
    SupplierName,
    Reason,
    Date,
    CreatedAt,
    UpdatedAt,
}

/// <summary>
/// Whitelisted sortable fields for the customer-facing menu endpoints. Named
/// after the menu DTO fields (<c>name</c>, <c>price</c>) and deliberately
/// excluding internal data such as cost prices.
/// </summary>
internal enum MenuSortField
{
    Id,
    Name,
    Price,
}

/// <summary>
/// Parses a comma-separated Django-style ordering (<c>name,-price</c>).
/// Matching is case-insensitive and ignores underscores, so both
/// <c>salePrice</c> and <c>sale_price</c> resolve. Unknown fields are
/// dropped; empty input yields no keys.
/// </summary>
internal static class OrderByParser
{
    internal static IReadOnlyList<(TField Field, bool Descending)> Parse<TField>(string? orderBy)
        where TField : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            return [];
        }

        var keys = new List<(TField, bool)>();
        foreach (var token in orderBy.Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var descending = token.StartsWith('-');
            var name = token.TrimStart('-', '+').Replace("_", string.Empty, StringComparison.Ordinal);
            if (Enum.TryParse<TField>(name, ignoreCase: true, out var field)
                && Enum.IsDefined(field))
            {
                keys.Add((field, descending));
            }
        }

        return keys;
    }
}

/// <summary>
/// Scalar selector for each whitelisted sort field. Boxed to
/// <c>object</c>; EF Core still translates these to SQL ORDER BY.
/// </summary>
internal static class SortSelectors
{
    internal static Expression<Func<ProductRecord, object>> Selector(this ProductSortField field) =>
        field switch
        {
            ProductSortField.Description => p => p.Description,
            ProductSortField.IsSupply => p => p.IsSupply,
            ProductSortField.CostPrice => p => p.CostPrice ?? 0,
            ProductSortField.SalePrice => p => p.SalePrice ?? 0,
            ProductSortField.MeasureUnit => p => p.MeasureUnit,
            ProductSortField.CreatedAt => p => p.CreatedAt,
            ProductSortField.UpdatedAt => p => p.UpdatedAt,
            _ => p => p.Id,
        };

    internal static Expression<Func<DishRecord, object>> Selector(this DishSortField field) =>
        field switch
        {
            DishSortField.Name => d => d.Name,
            DishSortField.Price => d => d.Price,
            DishSortField.CreatedAt => d => d.CreatedAt,
            DishSortField.UpdatedAt => d => d.UpdatedAt,
            _ => d => d.Id,
        };

    internal static Expression<Func<ComboRecord, object>> Selector(this ComboSortField field) =>
        field switch
        {
            ComboSortField.Name => c => c.Name,
            ComboSortField.Price => c => c.Price,
            ComboSortField.CreatedAt => c => c.CreatedAt,
            ComboSortField.UpdatedAt => c => c.UpdatedAt,
            _ => c => c.Id,
        };

    internal static Expression<Func<BranchRecord, object>> Selector(this BranchSortField field) =>
        field switch
        {
            BranchSortField.Name => b => b.Name,
            BranchSortField.Address => b => b.Address,
            BranchSortField.WarehouseOnly => b => b.WarehouseOnly,
            BranchSortField.CreatedAt => b => b.CreatedAt,
            BranchSortField.UpdatedAt => b => b.UpdatedAt,
            _ => b => b.Id,
        };

    internal static Expression<Func<UserRecord, object>> Selector(this UserSortField field) =>
        field switch
        {
            UserSortField.FirstName => u => u.FirstName,
            UserSortField.FatherLastName => u => u.FatherLastName,
            UserSortField.MotherLastName => u => u.MotherLastName!,
            UserSortField.Email => u => u.Email,
            UserSortField.CreatedAt => u => u.CreatedAt,
            UserSortField.UpdatedAt => u => u.UpdatedAt,
            _ => u => u.Id,
        };

    internal static Expression<Func<BranchStorageRecord, object>> Selector(this BranchStorageSortField field) =>
        field switch
        {
            BranchStorageSortField.ProductId => bs => bs.ProductId,
            BranchStorageSortField.Description => bs => bs.Product.Description,
            BranchStorageSortField.MinimumStock => bs => bs.MinimumStock,
            BranchStorageSortField.CurrentStock => bs => bs.CurrentStock,
            BranchStorageSortField.LastUpdated => bs => bs.UpdatedAt,
            BranchStorageSortField.CreatedAt => bs => bs.CreatedAt,
            BranchStorageSortField.UpdatedAt => bs => bs.UpdatedAt,
            _ => bs => bs.Id,
        };

    internal static Expression<Func<BranchStorageHistoryRecord, object>> Selector(this BranchStorageHistorySortField field) =>
        field switch
        {
            BranchStorageHistorySortField.Type => bsh => bsh.Type,
            BranchStorageHistorySortField.Folio => bsh => bsh.Folio ?? string.Empty,
            BranchStorageHistorySortField.SupplierName => bsh => bsh.SupplierName ?? string.Empty,
            BranchStorageHistorySortField.Reason => bsh => bsh.Reason ?? string.Empty,
            BranchStorageHistorySortField.Date => bsh => bsh.CreatedAt,
            BranchStorageHistorySortField.CreatedAt => bsh => bsh.CreatedAt,
            BranchStorageHistorySortField.UpdatedAt => bsh => bsh.UpdatedAt,
            _ => bsh => bsh.Id,
        };

    internal static Expression<Func<ProductRecord, object>> ProductSelector(this MenuSortField field) =>
        field switch
        {
            MenuSortField.Name => p => p.Description,
            MenuSortField.Price => p => p.SalePrice ?? 0,
            _ => p => p.Id,
        };

    internal static Expression<Func<DishRecord, object>> DishSelector(this MenuSortField field) =>
        field switch
        {
            MenuSortField.Name => d => d.Name,
            MenuSortField.Price => d => d.Price,
            _ => d => d.Id,
        };

    internal static Expression<Func<ComboRecord, object>> ComboSelector(this MenuSortField field) =>
        field switch
        {
            MenuSortField.Name => c => c.Name,
            MenuSortField.Price => c => c.Price,
            _ => c => c.Id,
        };
}

/// <summary>
/// Applies a parsed ordering to a query. With no valid keys the query
/// falls back to ascending id; otherwise the id is appended as the final
/// tiebreak so paging is deterministic.
/// </summary>
internal static class QueryableOrdering
{
    internal static IQueryable<T> ApplyOrdering<T>(
        this IQueryable<T> query,
        IReadOnlyList<(Expression<Func<T, object>> Selector, bool Descending)> keys,
        Expression<Func<T, object>> fallback)
    {
        if (keys.Count == 0)
        {
            return query.OrderBy(fallback);
        }

        IOrderedQueryable<T> ordered = keys[0].Descending
            ? query.OrderByDescending(keys[0].Selector)
            : query.OrderBy(keys[0].Selector);

        foreach (var (selector, descending) in keys.Skip(1))
        {
            ordered = descending
                ? ordered.ThenByDescending(selector)
                : ordered.ThenBy(selector);
        }

        return ordered.ThenBy(fallback);
    }
}
