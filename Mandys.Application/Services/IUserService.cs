using Mandys.DTOs;

namespace Mandys.Services;

public interface IUserService
{
    Task<UserResponse> GetCurrentUserAsync(Guid? currentUserId);

    Task<PagedUsersResponse> GetUsersAsync(int page, int pageSize, string? search, string? role);

    Task<UserResponse> GetUserByIdAsync(Guid id);

    Task<UserResponse> CreateUserAsync(CreateUserRequest request);

    Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request);

    Task DeleteUserAsync(Guid id, Guid? currentUserId);
}
