using Carter;
using Mandys.DTOs;
using Mandys.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

public class AuthHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", Login);

        app.MapPost("/refresh", Refresh);

        app.MapPost("/logout", Logout)
            .RequireAuthorization();
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        IAuthService authService,
        HttpContext context,
        bool useCookies = true)
    {
        try
        {
            var tokens = await authService.LoginAsync(request.Email, request.Password);

            if (!useCookies)
            {
                return Results.Ok(new TokenAuthResponse(
                    tokens.AccessToken,
                    tokens.RefreshToken,
                    "Bearer",
                    tokens.ExpiresInSeconds
                ));
            }

            context.Response.Cookies.Append(
                "access_token",
                tokens.AccessToken,
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
                tokens.RefreshToken,
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
        catch (ServiceException ex)
        {
            return MapAuthError(ex);
        }
    }

    private static async Task<IResult> Refresh(
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] RefreshRequest? request,
        IAuthService authService,
        HttpContext context,
        bool useCookies = true)
    {
        try
        {
            var refreshToken = useCookies
                ? request?.RefreshToken
                : context.Request.Cookies["refresh_token"];

            var tokens = await authService.RefreshAsync(refreshToken);

            if (!useCookies)
            {
                return Results.Ok(new TokenAuthResponse(
                    tokens.AccessToken,
                    tokens.RefreshToken,
                    "Bearer",
                    tokens.ExpiresInSeconds
                ));
            }

            context.Response.Cookies.Append(
                "access_token",
                tokens.AccessToken,
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
                tokens.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    MaxAge = TimeSpan.FromMinutes(60)
                });

            return Results.Ok();
        }
        catch (ServiceException ex)
        {
            return MapAuthError(ex);
        }
    }

    private static async Task<IResult> Logout(
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] RefreshRequest? request,
        IAuthService authService,
        HttpContext context,
        bool useCookies = true)
    {
        try
        {
            var refreshToken = useCookies
                ? request?.RefreshToken
                : context.Request.Cookies["refresh_token"];

            await authService.LogoutAsync(refreshToken);

            if (useCookies)
            {
                context.Response.Cookies.Delete("access_token");
                context.Response.Cookies.Delete("refresh_token");
            }

            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return MapAuthError(ex);
        }
    }

    private static IResult MapAuthError(ServiceException ex) =>
        ex.StatusCode == StatusCodes.Status400BadRequest
            ? Results.BadRequest(new { message = ex.Message })
            : Results.Unauthorized();
}
