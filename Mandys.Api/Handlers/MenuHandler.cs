using Carter;
using Mandys.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Api.Handlers;

public class MenuHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Public: anyone (even unauthenticated visitors) can browse the menu.
        var menuRoutes = app.MapGroup("/menu")
            .WithTags("Menu")
            .AllowAnonymous();

        menuRoutes.MapGet("/products", GetProducts);
        menuRoutes.MapGet("/dishes", GetDishes);
        menuRoutes.MapGet("/combos", GetCombos);
    }

    private static async Task<IResult> GetProducts(
        IMenuService menuService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null)
    {
        return Results.Ok(await menuService.GetProductsAsync(page, pageSize, search, orderBy));
    }

    private static async Task<IResult> GetDishes(
        IMenuService menuService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null)
    {
        return Results.Ok(await menuService.GetDishesAsync(page, pageSize, search, orderBy));
    }

    private static async Task<IResult> GetCombos(
        IMenuService menuService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null)
    {
        return Results.Ok(await menuService.GetCombosAsync(page, pageSize, search, orderBy));
    }
}
