using Carter;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Mandys.Api.Configuration;
using Mandys.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCarter(configurator: c => c.WithEmptyValidators());

builder.ConfigureDatabase();
builder.ConfigureStorage();
builder.ConfigureAuth();
builder.ConfigureUsers();
builder.ConfigureBranches();
builder.ConfigureProducts();
builder.ConfigureDishes();
builder.ConfigureCombos();
builder.ConfigureLogs();

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

app.MapCarter();

app.Run();
