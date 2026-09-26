using System.Security.Cryptography;
using System.Text;
using Carter;
using FluentValidation;
using Mandys.Configuration;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Mandys.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Mandys.Handlers;

public class AuthHandler : ICarterModule
{
    // Double-submit CSRF cookie. Named for axios's defaults
    // (xsrfCookieName / xsrfHeaderName), which then just work.
    private const string XsrfCookieName = "XSRF-TOKEN";
    private const string XsrfHeaderName = "X-XSRF-TOKEN";
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

            AppendXsrfCookie(context, jwtOptions.Value, cookieSettings.Value);

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

            if (useCookies && !HasValidCsrfToken(context, frontendOptions.Value))
            {
                return Results.BadRequest(new { message = "Invalid CSRF token." });
            }

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

            AppendXsrfCookie(context, jwtOptions.Value, cookieSettings.Value);

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

            if (useCookies && !HasValidCsrfToken(context, frontendOptions.Value))
            {
                return Results.BadRequest(new { message = "Invalid CSRF token." });
            }

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
        context.Response.Cookies.Delete(
            XsrfCookieName,
            new CookieOptions { Secure = secure, SameSite = sameSite, Path = "/" });
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

    private static void AppendXsrfCookie(HttpContext context, JwtOptions jwtOptions, AuthCookieSettings settings) =>
        context.Response.Cookies.Append(
            // Base64URL: cookie-safe alphabet, so the value needs no
            // URL-encoding and the JS-echoed header matches byte-for-byte.
            XsrfCookieName,
            WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32)),
            new CookieOptions
            {
                HttpOnly = false, // JS must read it to echo the header
                Secure = IsSecureCookie(context, settings),
                SameSite = ParseSameSite(settings.CookieSameSite),
                Path = "/",
                MaxAge = TimeSpan.FromMinutes(jwtOptions.RefreshTokenExpireMinutes),
            });

    private static bool HasValidCsrfToken(HttpContext context, FrontendOptions frontend)
    {
        // Same-site SPAs echo the double-submit cookie (axios does this
        // automatically). A cross-site SPA cannot read the backend's cookies
        // from JS, so fall back to the browser-controlled Origin/Referer,
        // which an attacker's site cannot spoof while carrying our cookies.
        if (HasValidXsrfToken(context))
            return true;

        var allowedOrigins = frontend.GetAllowedOrigins();
        var origin = context.Request.Headers.Origin.ToString();
        if (!string.IsNullOrEmpty(origin))
            return FrontendOptions.IsAllowedOrigin(origin, allowedOrigins);

        var referer = context.Request.Headers.Referer.ToString();
        if (string.IsNullOrEmpty(referer)
            || !Uri.TryCreate(referer, UriKind.Absolute, out var refererUri))
            return false;

        return FrontendOptions.IsAllowedOrigin(
            $"{refererUri.Scheme}://{refererUri.Authority}", allowedOrigins);
    }

    private static bool HasValidXsrfToken(HttpContext context)
    {
        var cookie = context.Request.Cookies[XsrfCookieName];
        var header = context.Request.Headers[XsrfHeaderName].ToString();

        return !string.IsNullOrEmpty(cookie)
            && !string.IsNullOrEmpty(header)
            && cookie.Length == header.Length
            && CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(cookie),
                Encoding.UTF8.GetBytes(header));
    }
}
