using Carter;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

using Mandys.Configuration;
using Mandys.Domain;
using Mandys;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCarter();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention());

builder.ConfigureJwt();

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

app.MapCarter();

app.Run();
