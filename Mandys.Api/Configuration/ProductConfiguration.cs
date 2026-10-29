using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class ProductConfiguration
{
    public static void ConfigureProducts(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IProductRepository, ProductRepository>();
        wab.Services.AddScoped<IProductService, ProductService>();

        wab.Services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        wab.Services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
    }
}
