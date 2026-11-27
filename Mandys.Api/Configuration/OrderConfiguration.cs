using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.Configuration;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class OrderConfiguration
{
    public static void ConfigureOrders(this WebApplicationBuilder wab)
    {
        wab.Services.Configure<LoyaltyOptions>(wab.Configuration.GetSection(LoyaltyOptions.SectionName));

        wab.Services.AddScoped<IOrderRepository, OrderRepository>();
        wab.Services.AddScoped<IOrderService, OrderService>();

        wab.Services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();
    }
}
