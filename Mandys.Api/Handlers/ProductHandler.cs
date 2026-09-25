using Carter;
using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

public class ProductHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var productRoutes = app.MapGroup("/products")
            .WithTags("Products")
            .RequireAuthorization();

        productRoutes.MapGet("", GetProducts);

        productRoutes.MapGet("/{id:int}", GetProductById);

        // Product catalog changes - restricted to Admin users only
        productRoutes.MapPost("", CreateProduct)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator));

        productRoutes.MapPut("/{id:int}", UpdateProduct)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator));

        productRoutes.MapDelete("/{id:int}", DeleteProduct)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator));
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
        IProductService productService)
    {
        try
        {
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
        IProductService productService)
    {
        try
        {
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
