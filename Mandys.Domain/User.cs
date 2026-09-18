namespace Mandys.Domain;

/// <summary>
/// Domain user. Guards its own invariants; persistence mapping lives in
/// Infrastructure.
/// </summary>
public class User : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string role,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
        : base(id)
    {
        FirstName = GuardName(firstName, nameof(firstName));
        LastName = GuardName(lastName, nameof(lastName));
        Email = GuardEmail(email);
        PasswordHash = GuardNotEmpty(passwordHash, nameof(passwordHash));
        Role = GuardRole(role);
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = GuardName(firstName, nameof(firstName));
        LastName = GuardName(lastName, nameof(lastName));
    }

    public void ChangeEmail(string email)
    {
        Email = GuardEmail(email);
    }

    public void SetRole(string role)
    {
        Role = GuardRole(role);
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = GuardNotEmpty(passwordHash, nameof(passwordHash));
    }

    private static string GuardNotEmpty(string value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{name} is required.", name)
            : value.Trim();

    private static string GuardName(string value, string name) =>
        GuardNotEmpty(value, name);

    private static string GuardEmail(string value) =>
        GuardNotEmpty(value, nameof(Email)).ToLowerInvariant();

    private static string GuardRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role) || !Roles.IsValid(role))
        {
            throw new ArgumentException(
                $"Invalid role '{role}'. Allowed roles: {string.Join(", ", Roles.All)}.",
                nameof(role));
        }

        return Roles.Normalize(role.Trim());
    }
}
