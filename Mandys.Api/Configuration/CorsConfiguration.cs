namespace Mandys.Configuration;

public static class CorsConfiguration
{
    public static void ConfigureCors(this WebApplicationBuilder wab, string policyName)
    {
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