namespace Mandys.Services;

/// <summary>
/// Thrown by application services for expected failures so handlers can map
/// them to HTTP responses.
/// </summary>
public sealed class ServiceException(int statusCode, string? message = null)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;

    public static ServiceException BadRequest(string message) => new(400, message);

    public static ServiceException Unauthorized() => new(401);

    public static ServiceException NotFound(string message) => new(404, message);

    public static ServiceException Conflict(string message) => new(409, message);
}
