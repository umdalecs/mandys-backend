using Mandys.Domain;

namespace Mandys.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
