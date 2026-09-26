using Carter;
using FluentValidation;
using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Mandys.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

public class ComboHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Combo catalog: franchise-wide and headquarters-managed, like dishes
        // and products. Writes are headquarters only (admins, ops chiefs).
        var comboRoutes = app.MapGroup("/combos")
            .WithTags("Combos")
            .RequireAuthorization();

        comboRoutes.MapGet("", GetCombos)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        comboRoutes.MapGet("/{id:int}", GetComboById)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        comboRoutes.MapPost("", CreateCombo)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        comboRoutes.MapPut("/{id:int}", UpdateCombo)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        comboRoutes.MapDelete("/{id:int}", DeleteCombo)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));
    }

    private static async Task<IResult> GetCombos(
        IComboService comboService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        return Results.Ok(await comboService.GetCombosAsync(page, pageSize, search));
    }

    private static async Task<IResult> GetComboById(
        int id,
        IComboService comboService)
    {
        try
        {
            return Results.Ok(await comboService.GetComboByIdAsync(id));
        }
        catch (ServiceException ex)
        {
            return MapComboError(ex);
        }
    }

    private static async Task<IResult> CreateCombo(
        [FromBody] CreateComboRequest request,
        IValidator<CreateComboRequest> validator,
        IComboService comboService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            var created = await comboService.CreateComboAsync(request);
            return Results.Created($"/combos/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapComboError(ex);
        }
    }

    private static async Task<IResult> UpdateCombo(
        int id,
        [FromBody] UpdateComboRequest request,
        IValidator<UpdateComboRequest> validator,
        IComboService comboService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            return Results.Ok(await comboService.UpdateComboAsync(id, request));
        }
        catch (ServiceException ex)
        {
            return MapComboError(ex);
        }
    }

    private static async Task<IResult> DeleteCombo(
        int id,
        IComboService comboService)
    {
        try
        {
            await comboService.DeleteComboAsync(id);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return MapComboError(ex);
        }
    }

    private static IResult MapComboError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
