using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
