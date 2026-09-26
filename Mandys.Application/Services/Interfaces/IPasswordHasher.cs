namespace Mandys.Services.Interfaces;

/// <summary>
/// Password hashing. Implemented by Infrastructure (Argon2id).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string passwordHash, string password);
}
