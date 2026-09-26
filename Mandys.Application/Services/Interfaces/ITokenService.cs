using Mandys.Domain;

namespace Mandys.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
