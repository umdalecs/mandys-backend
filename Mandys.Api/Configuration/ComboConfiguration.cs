using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class ComboConfiguration
{
    public static void ConfigureCombos(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IComboRepository, ComboRepository>();
        wab.Services.AddScoped<IComboService, ComboService>();

        wab.Services.AddScoped<IValidator<CreateComboRequest>, CreateComboRequestValidator>();
        wab.Services.AddScoped<IValidator<UpdateComboRequest>, UpdateComboRequestValidator>();
    }
}
