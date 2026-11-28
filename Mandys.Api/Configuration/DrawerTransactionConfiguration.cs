using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class DrawerConfiguration
{
    public static void ConfigureDrawerTransactions(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IDrawerTransactionRepository, DrawerTransactionRepository>();
        wab.Services.AddScoped<IDrawerReconciliationRepository, DrawerReconciliationRepository>();
        wab.Services.AddScoped<IDrawerService, DrawerService>();

        wab.Services.AddScoped<IValidator<CreateDrawerReconciliationRequest>, CreateDrawerReconciliationRequestValidator>();
        wab.Services.AddScoped<IValidator<ReconcileDrawerReasonRequest>, ReconcileDrawerReasonRequestValidator>();
    }
}
