using System.Security.Claims;
using System.Text;
using Mandys.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Mandys.Configuration;

public static class JwtConfiguration
{
    public static void ConfigureJwt(this WebApplicationBuilder wab)
    {
        wab.Services.Configure<JwtOptions>(wab.Configuration.GetRequiredSection(JwtOptions.SectionName));
        wab.Services.AddScoped<ITokenService, TokenService>();

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
    }
}