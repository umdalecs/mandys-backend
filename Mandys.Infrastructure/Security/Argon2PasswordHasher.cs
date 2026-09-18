using Isopoh.Cryptography.Argon2;
using Mandys.Services;

namespace Mandys.Infrastructure.Security;

public class Argon2PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => Argon2.Hash(password);

    public bool Verify(string passwordHash, string password) => Argon2.Verify(passwordHash, password);
}
