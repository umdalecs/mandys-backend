using Carter;
using Mandys.Application.Services;
using Mandys.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Api.Handlers;

public class MenuHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Public: anyone (even unauthenticated visitors) can browse a branch's menu.
        var menuRoutes = app.MapGroup("/branches/{branchId:int}/menu")
            .WithTags("Menu")
            .AllowAnonymous();

        menuRoutes.MapGet("/products", GetProducts);
        menuRoutes.MapGet("/dishes", GetDishes);
        menuRoutes.MapGet("/combos", GetCombos);
    }

    private static async Task<IResult> GetProducts(
        int branchId,
        IMenuService menuService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            return Results.Ok(await menuService.GetProductsAsync(branchId, page, pageSize, search, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapMenuError(ex);
        }
    }

    private static async Task<IResult> GetDishes(
        int branchId,
        IMenuService menuService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            return Results.Ok(await menuService.GetDishesAsync(branchId, page, pageSize, search, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapMenuError(ex);
        }
    }

    private static async Task<IResult> GetCombos(
        int branchId,
        IMenuService menuService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            return Results.Ok(await menuService.GetCombosAsync(branchId, page, pageSize, search, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapMenuError(ex);
        }
    }

    private static IResult MapMenuError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
