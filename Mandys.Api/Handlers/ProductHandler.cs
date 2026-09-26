using Carter;
using FluentValidation;
using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

public class ProductHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Supplies catalog: headquarters admins and branch warehouse chiefs.
        // RequireRole with several roles is OR, so any listed role grants access.
        // Policies stack with AND, so each route declares its full role set.
        var productRoutes = app.MapGroup("/products")
            .WithTags("Products")
            .RequireAuthorization();

        // Operations chiefs may only list the catalog, nothing else.
        productRoutes.MapGet("", GetProducts)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.WarehouseChief,
                Roles.OpChief));

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
        [FromQuery] string? search = null)
    {
        return Results.Ok(await productService.GetProductsAsync(page, pageSize, search));
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
