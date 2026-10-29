using Carter;
using FluentValidation;
using Mandys.Api.Validators;
using Mandys.Application.Configuration;
using Mandys.Application.DTOs;
using Mandys.Application.Services;
using Mandys.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mandys.Api.Handlers;

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
        IValidator<LoginRequest> validator,
        IAuthService authService,
        IOptions<JwtOptions> jwtOptions,
        IOptions<AuthCookieSettings> cookieSettings,
        HttpContext context,
        bool useCookies = true)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

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
                AuthCookieOptions(context, cookieSettings.Value, TimeSpan.FromMinutes(jwtOptions.Value.ExpireMinutes)));

            context.Response.Cookies.Append(
                "refresh_token",
                tokens.RefreshToken,
                AuthCookieOptions(context, cookieSettings.Value, TimeSpan.FromMinutes(jwtOptions.Value.RefreshTokenExpireMinutes)));

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
        IOptions<JwtOptions> jwtOptions,
        IOptions<AuthCookieSettings> cookieSettings,
        IOptions<FrontendOptions> frontendOptions,
        HttpContext context,
        bool useCookies = true)
    {
        try
        {
            var refreshToken = useCookies
                ? context.Request.Cookies["refresh_token"]
                : request?.RefreshToken;

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
                AuthCookieOptions(context, cookieSettings.Value, TimeSpan.FromMinutes(jwtOptions.Value.ExpireMinutes)));

            context.Response.Cookies.Append(
                "refresh_token",
                tokens.RefreshToken,
                AuthCookieOptions(context, cookieSettings.Value, TimeSpan.FromMinutes(jwtOptions.Value.RefreshTokenExpireMinutes)));

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
        IOptions<AuthCookieSettings> cookieSettings,
        IOptions<FrontendOptions> frontendOptions,
        HttpContext context,
        bool useCookies = true)
    {
        try
        {
            var refreshToken = useCookies
                ? context.Request.Cookies["refresh_token"]
                : request?.RefreshToken;

            await authService.LogoutAsync(refreshToken);

            if (useCookies)
            {
                DeleteAuthCookies(context, cookieSettings.Value);
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

    private static CookieOptions AuthCookieOptions(HttpContext context, AuthCookieSettings settings, TimeSpan maxAge) =>
        new()
        {
            HttpOnly = true,
            Secure = IsSecureCookie(context, settings),
            SameSite = ParseSameSite(settings.CookieSameSite),
            Path = "/",
            MaxAge = maxAge,
        };

    private static void DeleteAuthCookies(HttpContext context, AuthCookieSettings settings)
    {
        // Deletion only takes effect when Path/SameSite/Secure match the
        // cookies that were set; the defaults would leave cross-site
        // (None + Secure) cookies behind.
        var sameSite = ParseSameSite(settings.CookieSameSite);
        var secure = IsSecureCookie(context, settings);
        context.Response.Cookies.Delete(
            "access_token",
            new CookieOptions { HttpOnly = true, Secure = secure, SameSite = sameSite, Path = "/" });
        context.Response.Cookies.Delete(
            "refresh_token",
            new CookieOptions { HttpOnly = true, Secure = secure, SameSite = sameSite, Path = "/" });
    }

    private static SameSiteMode ParseSameSite(string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "none" => SameSiteMode.None,
            "strict" => SameSiteMode.Strict,
            _ => SameSiteMode.Lax,
        };

    private static bool IsSecureCookie(HttpContext context, AuthCookieSettings settings)
    {
        // Browsers reject SameSite=None without Secure, so it is forced.
        // Otherwise Secure follows HTTPS (auto) unless explicitly Always
        // (e.g. behind a proxy where IsHttps is fixed via ForwardedHeaders).
        if (ParseSameSite(settings.CookieSameSite) == SameSiteMode.None)
            return true;
        if (string.Equals(settings.CookieSecure, "Always", StringComparison.OrdinalIgnoreCase))
            return true;
        return context.Request.IsHttps;
    }
}
