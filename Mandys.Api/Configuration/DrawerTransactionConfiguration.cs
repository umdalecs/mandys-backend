using Mandys.Application.Services.Implementations;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Repositories;

namespace Mandys.Api.Configuration;

public static class DrawerConfiguration
{
    public static void ConfigureDrawerTransactions(this WebApplicationBuilder wab)
    {
        wab.Services.AddScoped<IDrawerTransactionRepository, DrawerTransactionRepository>();
        wab.Services.AddScoped<IDrawerService, DrawerService>();
    }
}
