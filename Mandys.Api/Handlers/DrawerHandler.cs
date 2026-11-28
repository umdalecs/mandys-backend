using System.Security.Claims;
using Carter;
using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.Common;
using Mandys.Application.DTOs;
using Mandys.Application.Services;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Api.Handlers;

public class DrawerHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var ownDrawerRoutes = app.MapGroup("/drawer")
            .WithTags("Drawer")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Cashier));

        ownDrawerRoutes.MapGet("/balance", GetOwnDrawerBalance);
        ownDrawerRoutes.MapGet("/transactions", GetOwnDrawerTransactions);

        var branchDrawerRoutes = app.MapGroup("/branches/{branchId:int}")
            .WithTags("DrawerTransactions")
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief,
                Roles.BranchChief));

        branchDrawerRoutes.MapGet("/drawers", GetBranchDrawers);
        branchDrawerRoutes.MapGet("/drawers/balances", GetBranchDrawerBalances);
        branchDrawerRoutes.MapGet("/drawers/{userId:int}/balance", GetBranchUserDrawerBalance);
        branchDrawerRoutes.MapGet("/drawer-transactions", GetBranchDrawerTransactions);

        var branchReconciliationRoutes = app.MapGroup("/branches/{branchId:int}")
            .WithTags("DrawerTransactions")
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.BranchChief));

        branchReconciliationRoutes.MapPost("/drawers/{userId:int}/reconciliation", CreateDrawerReconciliation);
        branchReconciliationRoutes.MapPut(
            "/drawers/{userId:int}/reconciliations/{reconciliationId:int}/reason",
            ReconcileDrawerDiscrepancy);
    }

    private static async Task<IResult> GetOwnDrawerBalance(
        ClaimsPrincipal claimsPrincipal,
        IDrawerService drawerService)
    {
        try
        {
            var (userId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            if (!userBranchId.HasValue)
            {
                throw ServiceException.BadRequest("El usuario actual no tiene una sucursal asignada.");
            }

            return Results.Ok(await drawerService.GetBalanceAsync(
                userBranchId.Value, userId, userId, userRole, userBranchId));
        }
        catch (ServiceException ex)
        {
            return MapError(ex);
        }
    }

    private static async Task<IResult> GetOwnDrawerTransactions(
        ClaimsPrincipal claimsPrincipal,
        IDrawerService drawerService,
        [FromQuery] string? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            var (userId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            if (!userBranchId.HasValue)
            {
                throw ServiceException.BadRequest("El usuario actual no tiene una sucursal asignada.");
            }

            return Results.Ok(await drawerService.GetTransactionsAsync(
                userBranchId.Value, userId, type, userId, userRole, userBranchId, page, pageSize, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapError(ex);
        }
    }

    private static async Task<IResult> GetBranchDrawers(
        int branchId,
        ClaimsPrincipal claimsPrincipal,
        IDrawerService drawerService)
    {
        try
        {
            var (_, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await drawerService.GetBranchDrawersAsync(branchId, userRole, userBranchId));
        }
        catch (ServiceException ex)
        {
            return MapError(ex);
        }
    }

    private static async Task<IResult> GetBranchDrawerBalances(
        int branchId,
        ClaimsPrincipal claimsPrincipal,
        IDrawerService drawerService)
    {
        try
        {
            var (_, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await drawerService.GetBranchBalancesAsync(branchId, userRole, userBranchId));
        }
        catch (ServiceException ex)
        {
            return MapError(ex);
        }
    }

    private static async Task<IResult> GetBranchUserDrawerBalance(
        int branchId,
        int userId,
        ClaimsPrincipal claimsPrincipal,
        IDrawerService drawerService)
    {
        try
        {
            var (requesterId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await drawerService.GetBalanceAsync(
                branchId, userId, requesterId, userRole, userBranchId));
        }
        catch (ServiceException ex)
        {
            return MapError(ex);
        }
    }

    private static async Task<IResult> GetBranchDrawerTransactions(
        int branchId,
        ClaimsPrincipal claimsPrincipal,
        IDrawerService drawerService,
        [FromQuery] int? userId = null,
        [FromQuery] string? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            var (requesterId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await drawerService.GetTransactionsAsync(
                branchId, userId, type, requesterId, userRole, userBranchId, page, pageSize, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapError(ex);
        }
    }

    private static async Task<IResult> CreateDrawerReconciliation(
        int branchId,
        int userId,
        [FromBody] CreateDrawerReconciliationRequest request,
        IValidator<CreateDrawerReconciliationRequest> validator,
        ClaimsPrincipal claimsPrincipal,
        IDrawerService drawerService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));
            var (requesterId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            var created = await drawerService.CreateReconciliationAsync(
                branchId, userId, request, requesterId, userRole, userBranchId);
            return Results.Created(
                $"/branches/{branchId}/drawers/{userId}/reconciliations/{created.Id}",
                created);
        }
        catch (ServiceException ex)
        {
            return MapError(ex);
        }
    }

    private static async Task<IResult> ReconcileDrawerDiscrepancy(
        int branchId,
        int userId,
        int reconciliationId,
        [FromBody] ReconcileDrawerReasonRequest request,
        IValidator<ReconcileDrawerReasonRequest> validator,
        ClaimsPrincipal claimsPrincipal,
        IDrawerService drawerService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));
            var (requesterId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            var updated = await drawerService.ReconcileDiscrepancyAsync(
                branchId, userId, reconciliationId, request, requesterId, userRole, userBranchId);
            return Results.Ok(updated);
        }
        catch (ServiceException ex)
        {
            return MapError(ex);
        }
    }

    private static (int UserId, string UserRole, int? UserBranchId) ExtractUserContext(ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.GetUserId()
            ?? throw ServiceException.Unauthorized();
        var userRole = claimsPrincipal.GetUserRole()
            ?? throw ServiceException.Unauthorized();
        var userBranchId = claimsPrincipal.GetBranchId();
        return (userId, userRole, userBranchId);
    }

    private static IResult MapError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
