using Mandys.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Api.Configuration;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this WebApplicationBuilder wab)
    {
        wab.Services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseNpgsql(wab.Configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());
    }
}
