using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class DishConfiguration
{
    public static void ConfigureDishes(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IDishRepository, DishRepository>();
        wab.Services.AddScoped<IDishService, DishService>();

        wab.Services.AddScoped<IValidator<CreateDishRequest>, CreateDishRequestValidator>();
        wab.Services.AddScoped<IValidator<UpdateDishRequest>, UpdateDishRequestValidator>();
    }
}
