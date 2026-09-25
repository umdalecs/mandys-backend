using System.Security.Claims;

namespace Mandys.Common;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        var idStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? principal.FindFirst("sub")?.Value;
        return int.TryParse(idStr, out var id) ? id : null;
    }

    public static string? GetUserRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value
               ?? principal.FindFirst("role")?.Value;
    }
}
