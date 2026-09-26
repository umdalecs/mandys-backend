using Carter;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

using Mandys.Configuration;
using Mandys.Domain;
using Mandys.Infrastructure.Persistence;
using Mandys.Infrastructure.Security;
using Mandys.Services;
using Mandys;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCarter();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IDishRepository, DishRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();

builder.ConfigureJwt();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IDishService, DishService>();

builder.Services.Configure<FrontendOptions>(builder.Configuration.GetSection(FrontendOptions.SectionName));
builder.Services.Configure<AuthCookieSettings>(builder.Configuration.GetSection(AuthCookieSettings.SectionName));

// Separate-server deployments sit behind TLS-terminating proxies, so honor
// X-Forwarded-Proto/For: cookie Secure flags and redirects depend on IsHttps.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddAuthorization(options =>
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

const string FrontendCorsPolicy = "Frontend";

builder.ConfigureCors(FrontendCorsPolicy);

var app = builder.Build();

app.UseForwardedHeaders();

app.UseCors(FrontendCorsPolicy);

if (app.Environment.IsDevelopment())
{
    // Frontend-dev flow: the lightweight dev container ships no SDK or
    // dotnet-ef, so the api applies pending migrations itself on startup.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api/")
    .MapCarter();

app.Run();
