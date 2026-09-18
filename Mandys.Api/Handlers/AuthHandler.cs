using System.Security.Cryptography;
using System.Text;
using Carter;
using Mandys.Configuration;
using Mandys.DTOs;
using Mandys.Services;
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
        IAuthService authService,
        IOptions<JwtOptions> jwtOptions,
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
                AuthCookieOptions(context, TimeSpan.FromMinutes(jwtOptions.Value.ExpireMinutes)));

            context.Response.Cookies.Append(
                "refresh_token",
                tokens.RefreshToken,
                AuthCookieOptions(context, TimeSpan.FromMinutes(jwtOptions.Value.RefreshTokenExpireMinutes)));

            AppendXsrfCookie(context, jwtOptions.Value);

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
        HttpContext context,
        bool useCookies = true)
    {
        try
        {
            var refreshToken = useCookies
                ? context.Request.Cookies["refresh_token"]
                : request?.RefreshToken;

            if (useCookies && !HasValidXsrfToken(context))
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
                AuthCookieOptions(context, TimeSpan.FromMinutes(jwtOptions.Value.ExpireMinutes)));

            context.Response.Cookies.Append(
                "refresh_token",
                tokens.RefreshToken,
                AuthCookieOptions(context, TimeSpan.FromMinutes(jwtOptions.Value.RefreshTokenExpireMinutes)));

            AppendXsrfCookie(context, jwtOptions.Value);

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
                ? context.Request.Cookies["refresh_token"]
                : request?.RefreshToken;

            if (useCookies && !HasValidXsrfToken(context))
            {
                return Results.BadRequest(new { message = "Invalid CSRF token." });
            }

            await authService.LogoutAsync(refreshToken);

            if (useCookies)
            {
                context.Response.Cookies.Delete("access_token");
                context.Response.Cookies.Delete("refresh_token");
                context.Response.Cookies.Delete(XsrfCookieName);
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

    private static CookieOptions AuthCookieOptions(HttpContext context, TimeSpan maxAge) =>
        new()
        {
            HttpOnly = true,
            // Secure only over HTTPS: browsers treat localhost as a secure
            // context, but plain-HTTP LAN testing would break otherwise.
            // SameSite stays Lax: correct for same-site frontends (different
            // localhost ports still count as same-site); a cross-domain
            // production SPA needs None + Secure instead.
            Secure = context.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            MaxAge = maxAge,
        };

    private static void AppendXsrfCookie(HttpContext context, JwtOptions jwtOptions) =>
        context.Response.Cookies.Append(
            // Base64URL: cookie-safe alphabet, so the value needs no
            // URL-encoding and the JS-echoed header matches byte-for-byte.
            XsrfCookieName,
            WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32)),
            new CookieOptions
            {
                HttpOnly = false, // JS must read it to echo the header
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                MaxAge = TimeSpan.FromMinutes(jwtOptions.RefreshTokenExpireMinutes),
            });

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
