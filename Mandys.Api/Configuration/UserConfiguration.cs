using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class UserConfiguration
{
    public static void ConfigureUsers(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IUserRepository, UserRepository>();
        wab.Services.AddScoped<IUserService, UserService>();
        wab.Services.AddScoped<ICustomerService, CustomerService>();

        wab.Services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        wab.Services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();
        wab.Services.AddScoped<IValidator<RegisterCustomerRequest>, RegisterCustomerRequestValidator>();
        wab.Services.AddScoped<IValidator<UpdateCustomerRequest>, UpdateCustomerRequestValidator>();
    }
}
