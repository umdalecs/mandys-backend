using Carter;
using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Api.Handlers;

public class ProductHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var productRoutes = app.MapGroup("/products")
            .WithTags("Products")
            .RequireAuthorization();

        productRoutes.MapGet("", GetProducts)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.CentralWarehouseChief,
                Roles.WarehouseChief,
                Roles.OpChief));

        productRoutes.MapGet("/measure-units", GetMeasureUnits);

        productRoutes.MapGet("/{id:int}", GetProductById)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.WarehouseChief));

        productRoutes.MapPost("", CreateProduct)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.WarehouseChief));

        productRoutes.MapPut("/{id:int}", UpdateProduct)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.WarehouseChief));

        productRoutes.MapDelete("/{id:int}", DeleteProduct)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.WarehouseChief));
    }

    private static async Task<IResult> GetProducts(
        IProductService productService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] bool? isSupply = null)
    {
        return Results.Ok(await productService.GetProductsAsync(page, pageSize, search, orderBy, isSupply));
    }

    private static IResult GetMeasureUnits()
    {
        var units = MeasureUnits.All
            .Select(u => new MeasureUnitResponse(
                Value: u,
                AllowsDecimals: MeasureUnits.AllowsDecimals(u),
                SupplyOnly: !MeasureUnits.IsAllowedForProduct(u, isSupply: false)))
            .ToList();

        return Results.Ok(units);
    }

    private static async Task<IResult> GetProductById(
        int id,
        IProductService productService)
    {
        try
        {
            return Results.Ok(await productService.GetProductByIdAsync(id));
        }
        catch (ServiceException ex)
        {
            return MapProductError(ex);
        }
    }

    private static async Task<IResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        IValidator<CreateProductRequest> validator,
        IProductService productService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            var created = await productService.CreateProductAsync(request);
            return Results.Created($"/products/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapProductError(ex);
        }
    }

    private static async Task<IResult> UpdateProduct(
        int id,
        [FromBody] UpdateProductRequest request,
        IValidator<UpdateProductRequest> validator,
        IProductService productService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            return Results.Ok(await productService.UpdateProductAsync(id, request));
        }
        catch (ServiceException ex)
        {
            return MapProductError(ex);
        }
    }

    private static async Task<IResult> DeleteProduct(
        int id,
        IProductService productService)
    {
        try
        {
            await productService.DeleteProductAsync(id);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return MapProductError(ex);
        }
    }

    private static IResult MapProductError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
