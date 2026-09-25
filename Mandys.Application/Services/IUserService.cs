using Mandys.DTOs;

namespace Mandys.Services;

public interface IUserService
{
    Task<UserResponse> GetCurrentUserAsync(int? currentUserId);
    Task<PagedUsersResponse> GetUsersAsync(int page, int pageSize, string? search, string? role);

    Task<UserResponse> GetUserByIdAsync(int id);

    Task<UserResponse> CreateUserAsync(CreateUserRequest request);

    Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);

    Task DeleteUserAsync(int id, int? currentUserId);
}
