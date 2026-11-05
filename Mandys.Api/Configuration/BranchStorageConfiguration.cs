using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class BranchStorageConfiguration
{
    public static void ConfigureBranchStorage(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IBranchStorageRepository, BranchStorageRepository>();
        wab.Services.AddScoped<IBranchStorageService, BranchStorageService>();

        wab.Services.AddScoped<IValidator<UpdateBranchStorageRequest>, UpdateBranchStorageRequestValidator>();
    }
}
