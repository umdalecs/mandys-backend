namespace Mandys.DTOs;

public record LoginRequest(
    string Email,
    string Password
);

public record RefreshRequest(
    string? RefreshToken = null
);

public record TokenAuthResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresIn
);
