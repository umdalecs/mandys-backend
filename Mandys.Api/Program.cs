using Carter;
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
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();

builder.ConfigureJwt();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin));
});

const string FrontendCorsPolicy = "Frontend";

builder.ConfigureCors(FrontendCorsPolicy);

var app = builder.Build();

app.UseCors(FrontendCorsPolicy);

if (app.Environment.IsDevelopment())
{
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
