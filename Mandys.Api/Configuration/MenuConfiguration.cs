using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class MenuConfiguration
{
    public static void ConfigureMenu(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IMenuRepository, MenuRepository>();
        wab.Services.AddScoped<IMenuService, MenuService>();
    }
}
