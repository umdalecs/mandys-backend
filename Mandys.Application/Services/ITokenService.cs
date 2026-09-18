using Mandys.Entities;

namespace Mandys.Services;

public interface ITokenService
{
    string GenerateToken(UserEntity user);
    string GenerateRefreshToken();
}
