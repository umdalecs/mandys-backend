using Mandys.Domain;

namespace Mandys.DTOs;

/// <summary>
/// Customer view of a user row. Named separately from
/// <see cref="UserMapper.ToResponse"/> so the two projections of the same
/// entity stay unambiguous at call sites.
/// </summary>
public record CustomerResponse(
    int Id,
    string FirstName,
    string LastName,
    string? Email,
    bool HasLogin,
    DateTime? BannedAt
);

public record PagedCustomersResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<CustomerResponse> Items
);

/// <summary>
/// Self-registration of a point-of-sale customer. Deliberately carries no
/// role and no branch: those are assigned by the server, never by the
/// caller, so nobody can register themselves as staff.
/// </summary>
public record RegisterCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password
);

/// <summary>
/// A counter customer created on someone else's behalf (walk-in, or a
/// customer who does not want an account). No password, so the row has no
/// login credentials. An email is optional but worth capturing: it is what
/// lets the customer be found again to reclaim stored points.
/// </summary>
public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string? Email = null
);

/// <summary>
/// Counter-side edits to a customer. Credentials are not editable here;
/// passwords belong to the user module.
/// </summary>
public record UpdateCustomerRequest(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null
);

public static class CustomerMapper
{
    public static CustomerResponse ToCustomerResponse(this User user) =>
        new(
            user.ID,
            user.FirstName,
            user.LastName,
            user.Email,
            user.HasLogin,
            user.BannedAt
        );
}
