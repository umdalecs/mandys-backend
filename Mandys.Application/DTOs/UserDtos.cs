using Mandys.Domain;

namespace Mandys.DTOs;

public record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string? Email,
    string Role,
    int? BranchId,
    bool HasLogin
);

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string? Email = null,
    string? UserName = null,
    string? Password = null,
    string? Role = null,
    int? BranchId = null
);

public record UpdateUserRequest(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? UserName = null,
    string? Password = null,
    string? Role = null,
    int? BranchId = null,
    bool ClearBranch = false
);

public record PagedUsersResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<UserResponse> Items
);

public static class UserMapper
{
    public static UserResponse ToResponse(this User user) =>
        new(
            user.ID,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            user.BranchId,
            user.HasLogin
        );
}
