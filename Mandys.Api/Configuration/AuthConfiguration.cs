using System.Security.Claims;
using System.Text;
using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.Configuration;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Repositories;
using Mandys.Infrastructure.Security;
using Mandys.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Mandys.Api.Configuration;

public static class AuthConfiguration
{
    public static void ConfigureAuth(this WebApplicationBuilder wab)
    {
        wab.Services.Configure<JwtOptions>(wab.Configuration.GetRequiredSection(JwtOptions.SectionName));
        wab.Services.Configure<AuthCookieSettings>(wab.Configuration.GetSection(AuthCookieSettings.SectionName));

        wab.Services.AddScoped<ITokenService, TokenService>();
        wab.Services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        wab.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        wab.Services.AddScoped<IAuthService, AuthService>();
        wab.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        var jwtKey = wab.Configuration["Jwt:Key"];
        var jwtIssuer = wab.Configuration["Jwt:Issuer"];
        var jwtAudience = wab.Configuration["Jwt:Audience"];

        wab.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Prefer the cookie, but leave header-based Bearer
                        // tokens alone when no cookie is present.
                        if (context.Request.Cookies.TryGetValue("access_token", out var token)
                            && !string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        wab.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Administrator", policy => policy.RequireRole(Roles.Administrator));
            options.AddPolicy("OpChief", policy => policy.RequireRole(Roles.OpChief));
            options.AddPolicy("CentralWarehouseChief", policy => policy.RequireRole(Roles.CentralWarehouseChief));
            options.AddPolicy("WarehouseChief", policy => policy.RequireRole(Roles.WarehouseChief));
            options.AddPolicy("Cashier", policy => policy.RequireRole(Roles.Cashier));
            options.AddPolicy("Customer", policy => policy.RequireRole(Roles.Customer));
            options.AddPolicy("KitchenChief", policy => policy.RequireRole(Roles.KitchenChief));
            options.AddPolicy("BranchChief", policy => policy.RequireRole(Roles.BranchChief));
        });
    }
}
