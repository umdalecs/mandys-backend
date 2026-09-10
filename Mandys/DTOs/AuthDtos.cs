using FluentValidation;

namespace Mandys.DTOs;

public record LoginRequest(
    string? Email = null,
    string Password = ""
);

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(lr => lr.Email)
            .NotNull()
            .EmailAddress()
            .WithMessage("Invalid Email");

        RuleFor(lr => lr.Password)
            .MinimumLength(6)
            .WithMessage("Password try is too short");
    }
}

public record RefreshRequest(
    string? RefreshToken = null
);

public record TokenAuthResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresIn
);
