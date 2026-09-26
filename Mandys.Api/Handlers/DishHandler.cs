using Carter;
using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

public class DishHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Menu catalog: franchise-wide and headquarters-managed. Reads serve
        // everyone operating sales (cashiers sell, kitchen prepares, branch
        // chiefs manage); writes are headquarters only (admins, ops chiefs).
        var dishRoutes = app.MapGroup("/dishes")
            .WithTags("Dishes")
            .RequireAuthorization();

        dishRoutes.MapGet("", GetDishes)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));
        
        //     ,
        // Roles.BranchChief,
        // Roles.Cashier,
        // Roles.KitchenChief

        dishRoutes.MapGet("/{id:int}", GetDishById)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));
        
        //     ,
        // Roles.BranchChief,
        // Roles.Cashier,
        // Roles.KitchenChief

        dishRoutes.MapPost("", CreateDish)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        dishRoutes.MapPut("/{id:int}", UpdateDish)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        dishRoutes.MapDelete("/{id:int}", DeleteDish)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));
    }

    private static async Task<IResult> GetDishes(
        IDishService dishService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        return Results.Ok(await dishService.GetDishesAsync(page, pageSize, search));
    }

    private static async Task<IResult> GetDishById(
        int id,
        IDishService dishService)
    {
        try
        {
            return Results.Ok(await dishService.GetDishByIdAsync(id));
        }
        catch (ServiceException ex)
        {
            return MapDishError(ex);
        }
    }

    private static async Task<IResult> CreateDish(
        [FromBody] CreateDishRequest request,
        IDishService dishService)
    {
        try
        {
            var created = await dishService.CreateDishAsync(request);
            return Results.Created($"/dishes/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapDishError(ex);
        }
    }

    private static async Task<IResult> UpdateDish(
        int id,
        [FromBody] UpdateDishRequest request,
        IDishService dishService)
    {
        try
        {
            return Results.Ok(await dishService.UpdateDishAsync(id, request));
        }
        catch (ServiceException ex)
        {
            return MapDishError(ex);
        }
    }

    private static async Task<IResult> DeleteDish(
        int id,
        IDishService dishService)
    {
        try
        {
            await dishService.DeleteDishAsync(id);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return MapDishError(ex);
        }
    }

    private static IResult MapDishError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
