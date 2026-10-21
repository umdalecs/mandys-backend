using Mandys.Domain;

namespace Mandys.Application.DTOs;

public record UserResponse(
    int Id,
    string FirstName,
    string FatherLastName,
    string? MotherLastName,
    string Email,
    string Role,
    int? BranchId
);

public record CreateUserRequest(
    string FirstName,
    string FatherLastName,
    string Email,
    string Password,
    string? MotherLastName = null,
    string? Role = null,
    int? BranchId = null
);

public record UpdateUserRequest(
    string? FirstName = null,
    string? FatherLastName = null,
    string? MotherLastName = null,
    string? Email = null,
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
            user.FatherLastName,
            user.MotherLastName,
            user.Email,
            user.Role,
            user.BranchId
        );
}
