namespace Mandys.Domain;

/// <summary>
/// Domain user. Guards its own invariants; persistence mapping lives in
/// Infrastructure.
/// </summary>
public class User
{
    public int ID { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    /// <summary>
    /// Branch the user belongs to (many users to one branch). Optional for
    /// administrators and customers, who can use any branch; required for
    /// every other role.
    /// </summary>
    public int? BranchId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User(
        int id,
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string role,
        int? branchId = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        ID = id;
        FirstName = GuardName(firstName, nameof(firstName));
        LastName = GuardName(lastName, nameof(lastName));
        Email = GuardEmail(email);
        PasswordHash = GuardNotEmpty(passwordHash, nameof(passwordHash));
        Role = GuardRole(role);
        BranchId = GuardBranch(Role, branchId);
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
        var normalized = GuardRole(role);
        if (Roles.RequiresBranch(normalized) && BranchId is null)
        {
            throw new ArgumentException(
                $"Role '{normalized}' requires a branch. Assign one before changing the role.",
                nameof(role));
        }

        Role = normalized;
    }

    public void SetBranch(int? branchId)
    {
        BranchId = GuardBranch(Role, branchId);
    }

    public void ClearBranch() => SetBranch(null);

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

    private static int? GuardBranch(string role, int? branchId)
    {
        if (branchId.HasValue && branchId.Value <= 0)
        {
            throw new ArgumentException("BranchId must be a positive id.", nameof(branchId));
        }

        if (!branchId.HasValue && Roles.RequiresBranch(role))
        {
            throw new ArgumentException(
                $"Role '{role}' requires a branch.",
                nameof(branchId));
        }

        return branchId;
    }
}
