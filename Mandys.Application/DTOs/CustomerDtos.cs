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
    string? Email
);

public record PagedCustomersResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<CustomerResponse> Items
);

public record RegisterCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password
);

public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string? Email = null
);

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
            user.Email
        );
}
