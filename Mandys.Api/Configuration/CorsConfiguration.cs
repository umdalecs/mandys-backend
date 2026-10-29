using Mandys.Application.Configuration;
using Microsoft.AspNetCore.HttpOverrides;

namespace Mandys.Api.Configuration;

public static class CorsConfiguration
{
    public static void ConfigureCors(this WebApplicationBuilder wab, string policyName)
    {
        wab.Services.Configure<FrontendOptions>(wab.Configuration.GetSection(FrontendOptions.SectionName));

        // Separate-server deployments sit behind TLS-terminating proxies, so honor
        // X-Forwarded-Proto/For: cookie Secure flags and redirects depend on IsHttps.
        wab.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        var allowedOrigins = wab.Configuration
            .GetSection(FrontendOptions.SectionName)
            .Get<FrontendOptions>()?.GetAllowedOrigins()
            ?? FrontendOptions.DefaultOrigins;

        wab.Services.AddCors(options =>
        {
            options.AddPolicy(policyName, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .SetIsOriginAllowed(origin =>
                        FrontendOptions.IsAllowedOrigin(origin, allowedOrigins))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}
