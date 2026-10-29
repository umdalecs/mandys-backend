using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class LogConfiguration
{
    public static void ConfigureLogs(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<ILogRepository, LogRepository>();
        wab.Services.AddScoped<ILogService, LogService>();

        wab.Services.AddScoped<IValidator<CreateLogRequest>, CreateLogRequestValidator>();
    }
}
