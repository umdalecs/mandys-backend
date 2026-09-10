using Carter;
using Isopoh.Cryptography.Argon2;
using Mandys.Configuration;
using Mandys.DTOs;
using Mandys.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Mandys.Handlers;

public class AuthHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", Login)
            .WithName("AuthLogin")
            .WithSummary("Login with credentials and receive access and refresh tokens");

        app.MapPost("/refresh", Refresh)
            .WithName("AuthRefresh")
            .WithSummary("Refresh access token using a valid refresh token");
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        ApplicationDbContext db,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions,
        HttpContext context,
        bool useCookies = true)
    {
        var validator = new LoginRequestValidator();

        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            return Results.BadRequest(
                new { message = result.Errors.First().ErrorMessage }
                );
        }

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email!.ToLower());

        if (user is null || !Argon2.Verify(user.Password, request.Password))
        {
            return Results.Unauthorized();
        }

        var accessToken = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpireMinutes);

        await db.SaveChangesAsync();

        var expiresInSeconds = jwtOptions.Value.ExpireMinutes * 60;

        if (!useCookies)
        {
            return Results.Ok(new TokenAuthResponse(
                accessToken,
                refreshToken,
                "Bearer",
                expiresInSeconds
            ));
        }

        context.Response.Cookies.Append(
            "access_token",
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                MaxAge = TimeSpan.FromMinutes(15)
            });

        context.Response.Cookies.Append(
            "refresh_token",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/auth",
                MaxAge = TimeSpan.FromDays(30)
            });

        return Results.Ok();
    }

    private static async Task<IResult> Refresh(
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] RefreshRequest? request,
        HttpContext httpContext,
        ApplicationDbContext db,
        ITokenService tokenService,
        HttpContext context,
        IOptions<JwtOptions> jwtOptions,
        bool useCookies = true)
    {
        var refreshToken = request?.RefreshToken;

        if (useCookies)
        {
            refreshToken = context.Request.Cookies["access_token"];
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Results.BadRequest(new { message = "Refresh token is required." });
        }

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null || user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Results.Unauthorized();
        }

        var newAccessToken = tokenService.GenerateToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpireMinutes);

        await db.SaveChangesAsync();

        var expiresInSeconds = jwtOptions.Value.ExpireMinutes * 60;

        if (!useCookies)
        {
            return Results.Ok(new TokenAuthResponse(
                newAccessToken,
                refreshToken,
                "Bearer",
                expiresInSeconds
            ));
        }

        context.Response.Cookies.Append(
            "access_token",
            newAccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                MaxAge = TimeSpan.FromMinutes(10)
            });

        context.Response.Cookies.Append(
            "refresh_token",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/auth",
                MaxAge = TimeSpan.FromMinutes(60)
            });

        return Results.Ok();
    }
}