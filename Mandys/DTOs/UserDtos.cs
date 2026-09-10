using Mandys.Entities;

namespace Mandys.DTOs;

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role
);

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string? Role = null
);

public record UpdateUserRequest(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? UserName = null,
    string? Password = null,
    string? Role = null
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
    public static UserResponse ToResponse(this UserEntity user) =>
        new(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role
        );
}
