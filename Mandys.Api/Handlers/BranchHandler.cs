using Carter;
using FluentValidation;
using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Mandys.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

public class BranchHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Branches are the franchise structure every operational flow is
        // scoped to, so any authenticated user may read them (branch pickers
        // on the user form, the caller's own branch). Changing them is
        // headquarters only: administrators and operations chiefs.
        // RequireRole with several roles is OR, so any listed role grants access.
        // Policies stack with AND, so each route declares its full role set.
        var branchRoutes = app.MapGroup("/branches")
            .WithTags("Branches")
            .RequireAuthorization();

        branchRoutes.MapGet("", GetBranches);

        branchRoutes.MapGet("/{id:int}", GetBranchById);

        branchRoutes.MapPost("", CreateBranch)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        branchRoutes.MapPut("/{id:int}", UpdateBranch)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        branchRoutes.MapDelete("/{id:int}", DeleteBranch)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));
    }

    private static async Task<IResult> GetBranches(
        IBranchService branchService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        return Results.Ok(await branchService.GetBranchesAsync(page, pageSize, search));
    }

    private static async Task<IResult> GetBranchById(
        int id,
        IBranchService branchService)
    {
        try
        {
            return Results.Ok(await branchService.GetBranchByIdAsync(id));
        }
        catch (ServiceException ex)
        {
            return MapBranchError(ex);
        }
    }

    private static async Task<IResult> CreateBranch(
        [FromBody] CreateBranchRequest request,
        IValidator<CreateBranchRequest> validator,
        IBranchService branchService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            var created = await branchService.CreateBranchAsync(request);
            return Results.Created($"/branches/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapBranchError(ex);
        }
    }

    private static async Task<IResult> UpdateBranch(
        int id,
        [FromBody] UpdateBranchRequest request,
        IValidator<UpdateBranchRequest> validator,
        IBranchService branchService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            return Results.Ok(await branchService.UpdateBranchAsync(id, request));
        }
        catch (ServiceException ex)
        {
            return MapBranchError(ex);
        }
    }

    private static async Task<IResult> DeleteBranch(
        int id,
        IBranchService branchService)
    {
        try
        {
            await branchService.DeleteBranchAsync(id);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return MapBranchError(ex);
        }
    }

    private static IResult MapBranchError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
