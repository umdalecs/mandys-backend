using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class BranchConfiguration
{
    public static void ConfigureBranches(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IBranchRepository, BranchRepository>();
        wab.Services.AddScoped<IBranchService, BranchService>();

        wab.Services.AddScoped<IValidator<CreateBranchRequest>, CreateBranchRequestValidator>();
        wab.Services.AddScoped<IValidator<UpdateBranchRequest>, UpdateBranchRequestValidator>();
    }
}
