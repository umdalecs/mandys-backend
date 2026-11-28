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

public class OrderHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var branchOrderRoutes = app.MapGroup("/branches/{branchId:int}/orders")
            .WithTags("Orders")
            .RequireAuthorization();

        branchOrderRoutes.MapPost("", CreateBranchOrder)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Customer));

        branchOrderRoutes.MapGet("", GetBranchOrders)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief,
                Roles.BranchChief));

        branchOrderRoutes.MapGet("/{orderId:int}", GetBranchOrderById)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief,
                Roles.BranchChief));

        var orderRoutes = app.MapGroup("/orders")
            .WithTags("Orders")
            .RequireAuthorization();

        orderRoutes.MapPost("", CreateOwnBranchOrder)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Cashier));

        orderRoutes.MapGet("", GetOrders)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Cashier,
                Roles.KitchenChief));

        orderRoutes.MapGet("/{orderId:int}", GetOrderById)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Cashier,
                Roles.KitchenChief));

        orderRoutes.MapPatch("/{orderId:int}/completed", MarkOrderCompleted)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.KitchenChief));

        orderRoutes.MapPatch("/{orderId:int}/uncompleted", MarkOrderUncompleted)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.KitchenChief));
    }

    private static async Task<IResult> CreateBranchOrder(
        int branchId,
        ClaimsPrincipal claimsPrincipal,
        [FromBody] CreateOrderRequest request,
        IValidator<CreateOrderRequest> validator,
        IOrderService orderService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            var (userId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            var created = await orderService.CreateOrderAsync(
                branchId, userId, userRole, userBranchId, request);

            return Results.Created($"/branches/{branchId}/orders/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapOrderError(ex);
        }
    }

    private static async Task<IResult> CreateOwnBranchOrder(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] CreateOrderRequest request,
        IValidator<CreateOrderRequest> validator,
        IOrderService orderService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            var (userId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            if (!userBranchId.HasValue)
            {
                throw ServiceException.BadRequest("El usuario actual no tiene una sucursal asignada.");
            }

            var created = await orderService.CreateOrderAsync(
                userBranchId.Value, userId, userRole, userBranchId, request);

            return Results.Created($"/orders/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapOrderError(ex);
        }
    }

    private static async Task<IResult> GetBranchOrders(
        int branchId,
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? paymentMethod = null,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            var (userId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await orderService.GetOrdersAsync(
                branchId, userId, userRole, userBranchId, paymentMethod, OrderStatuses.Ready, page, pageSize, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapOrderError(ex);
        }
    }

    private static async Task<IResult> GetBranchOrderById(
        int branchId,
        int orderId,
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService)
    {
        try
        {
            var (userId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await orderService.GetOrderByIdAsync(
                branchId, orderId, userId, userRole, userBranchId));
        }
        catch (ServiceException ex)
        {
            return MapOrderError(ex);
        }
    }

    private static async Task<IResult> GetOrders(
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService,
        [FromQuery] int? branchId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? paymentMethod = null,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            var (userId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await orderService.GetOrdersAsync(
                branchId, userId, userRole, userBranchId, paymentMethod, OrderStatuses.InPreparation, page, pageSize, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapOrderError(ex);
        }
    }

    private static async Task<IResult> GetOrderById(
        int orderId,
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService)
    {
        try
        {
            var (userId, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await orderService.GetOrderByIdAsync(
                null, orderId, userId, userRole, userBranchId));
        }
        catch (ServiceException ex)
        {
            return MapOrderError(ex);
        }
    }

    private static async Task<IResult> MarkOrderCompleted(
        int orderId,
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService)
    {
        try
        {
            var (_, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await orderService.MarkOrderCompletedAsync(
                orderId, userRole, userBranchId));
        }
        catch (ServiceException ex)
        {
            return MapOrderError(ex);
        }
    }

    private static async Task<IResult> MarkOrderUncompleted(
        int orderId,
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService)
    {
        try
        {
            var (_, userRole, userBranchId) = ExtractUserContext(claimsPrincipal);
            return Results.Ok(await orderService.MarkOrderUncompletedAsync(
                orderId, userRole, userBranchId));
        }
        catch (ServiceException ex)
        {
            return MapOrderError(ex);
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

    private static IResult MapOrderError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
