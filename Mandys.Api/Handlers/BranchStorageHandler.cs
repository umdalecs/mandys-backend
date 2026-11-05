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

public class BranchStorageHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var storageRoutes = app.MapGroup("/branches/{branchId:int}/storage")
            .WithTags("BranchStorage")
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.OpChief));

        storageRoutes.MapGet("", GetBranchStorage);
        storageRoutes.MapGet("/{productId:int}", GetBranchStorageByProduct);
        storageRoutes.MapPut("/{productId:int}", UpdateBranchStorage);
        storageRoutes.MapPost("/in", CreateBranchStorageIn);
        storageRoutes.MapPost("/out", CreateBranchStorageOut);
        storageRoutes.MapGet("/history", GetBranchStorageHistory);
        storageRoutes.MapGet("/history/{historyId:int}", GetBranchStorageHistoryById);

        var ownStorageRoutes = app.MapGroup("/storage")
            .WithTags("BranchStorage")
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.WarehouseChief,
                Roles.CentralWarehouseChief));

        ownStorageRoutes.MapGet("", GetOwnBranchStorage);
        ownStorageRoutes.MapGet("/{productId:int}", GetOwnBranchStorageByProduct);
        ownStorageRoutes.MapPut("/{productId:int}", UpdateOwnBranchStorage);
        ownStorageRoutes.MapPost("/in", CreateOwnBranchStorageIn);
        ownStorageRoutes.MapPost("/out", CreateOwnBranchStorageOut);
        ownStorageRoutes.MapGet("/history", GetOwnBranchStorageHistory);
        ownStorageRoutes.MapGet("/history/{historyId:int}", GetOwnBranchStorageHistoryById);
    }

    private static async Task<IResult> GetBranchStorage(
        int branchId,
        IBranchStorageService branchStorageService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] bool? isSupply = null)
    {
        try
        {
            return Results.Ok(await branchStorageService.GetBranchStorageAsync(
                branchId, page, pageSize, search, orderBy, isSupply));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> GetBranchStorageByProduct(
        int branchId,
        int productId,
        IBranchStorageService branchStorageService)
    {
        try
        {
            return Results.Ok(await branchStorageService.GetBranchStorageByProductAsync(branchId, productId));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> UpdateBranchStorage(
        int branchId,
        int productId,
        [FromBody] UpdateBranchStorageRequest request,
        IValidator<UpdateBranchStorageRequest> validator,
        IBranchStorageService branchStorageService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            return Results.Ok(await branchStorageService.UpdateMinimumStockAsync(branchId, productId, request));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> CreateBranchStorageIn(
        int branchId,
        [FromBody] CreateStorageInRequest request,
        IValidator<CreateStorageInRequest> validator,
        IBranchStorageService branchStorageService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(await validator.ValidateAsync(request));

            var created = await branchStorageService.CreateInMovementAsync(branchId, request);
            return Results.Created($"/branches/{branchId}/storage/history/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> CreateBranchStorageOut(
        int branchId,
        [FromBody] CreateStorageOutRequest request,
        IValidator<CreateStorageOutRequest> validator,
        IBranchStorageService branchStorageService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(await validator.ValidateAsync(request));

            var created = await branchStorageService.CreateOutMovementAsync(branchId, request);
            return Results.Created($"/branches/{branchId}/storage/history/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> GetBranchStorageHistory(
        int branchId,
        IBranchStorageService branchStorageService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? type = null,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            return Results.Ok(await branchStorageService.GetBranchStorageHistoryAsync(
                branchId, page, pageSize, search, type, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> GetBranchStorageHistoryById(
        int branchId,
        int historyId,
        IBranchStorageService branchStorageService)
    {
        try
        {
            return Results.Ok(await branchStorageService.GetBranchStorageHistoryByIdAsync(branchId, historyId));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> GetOwnBranchStorage(
        ClaimsPrincipal claimsPrincipal,
        IBranchStorageService branchStorageService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] bool? isSupply = null)
    {
        try
        {
            var branchId = RequireBranchId(claimsPrincipal);
            return Results.Ok(await branchStorageService.GetBranchStorageAsync(
                branchId, page, pageSize, search, orderBy, isSupply));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> GetOwnBranchStorageByProduct(
        int productId,
        ClaimsPrincipal claimsPrincipal,
        IBranchStorageService branchStorageService)
    {
        try
        {
            var branchId = RequireBranchId(claimsPrincipal);
            return Results.Ok(await branchStorageService.GetBranchStorageByProductAsync(branchId, productId));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> UpdateOwnBranchStorage(
        int productId,
        ClaimsPrincipal claimsPrincipal,
        [FromBody] UpdateBranchStorageRequest request,
        IValidator<UpdateBranchStorageRequest> validator,
        IBranchStorageService branchStorageService)
    {
        try
        {
            var branchId = RequireBranchId(claimsPrincipal);
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            return Results.Ok(await branchStorageService.UpdateMinimumStockAsync(branchId, productId, request));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> CreateOwnBranchStorageIn(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] CreateStorageInRequest request,
        IValidator<CreateStorageInRequest> validator,
        IBranchStorageService branchStorageService)
    {
        try
        {
            var branchId = RequireBranchId(claimsPrincipal);
            RequestValidation.ThrowIfInvalid(await validator.ValidateAsync(request));

            var created = await branchStorageService.CreateInMovementAsync(branchId, request);
            return Results.Created($"/storage/history/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> CreateOwnBranchStorageOut(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] CreateStorageOutRequest request,
        IValidator<CreateStorageOutRequest> validator,
        IBranchStorageService branchStorageService)
    {
        try
        {
            var branchId = RequireBranchId(claimsPrincipal);
            RequestValidation.ThrowIfInvalid(await validator.ValidateAsync(request));

            var created = await branchStorageService.CreateOutMovementAsync(branchId, request);
            return Results.Created($"/storage/history/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> GetOwnBranchStorageHistory(
        ClaimsPrincipal claimsPrincipal,
        IBranchStorageService branchStorageService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? type = null,
        [FromQuery] string? orderBy = null)
    {
        try
        {
            var branchId = RequireBranchId(claimsPrincipal);
            return Results.Ok(await branchStorageService.GetBranchStorageHistoryAsync(
                branchId, page, pageSize, search, type, orderBy));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static async Task<IResult> GetOwnBranchStorageHistoryById(
        int historyId,
        ClaimsPrincipal claimsPrincipal,
        IBranchStorageService branchStorageService)
    {
        try
        {
            var branchId = RequireBranchId(claimsPrincipal);
            return Results.Ok(await branchStorageService.GetBranchStorageHistoryByIdAsync(branchId, historyId));
        }
        catch (ServiceException ex)
        {
            return MapBranchStorageError(ex);
        }
    }

    private static int RequireBranchId(ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.GetBranchId()
        ?? throw ServiceException.BadRequest("El usuario actual no tiene una sucursal asignada.");

    private static IResult MapBranchStorageError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
