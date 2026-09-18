namespace Mandys.Configuration;

public static class CorsConfiguration
{
    public static void ConfigureCors(this WebApplicationBuilder wab, string policyName)
    {
        wab.Services.AddCors(options =>
        {
            options.AddPolicy(policyName, policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:5173",
                        "http://127.0.0.1:5173")
                    .SetIsOriginAllowed(origin =>
                    {
                        if (string.IsNullOrEmpty(origin)) return false;
                        var host = new Uri(origin).Host;
                        return host is "localhost" or "127.0.0.1";
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}