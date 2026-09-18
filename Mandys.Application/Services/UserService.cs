using Isopoh.Cryptography.Argon2;
using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Services;

public class UserService(IApplicationDbContext db) : IUserService
{
    public async Task<UserResponse> GetCurrentUserAsync(Guid? currentUserId)
    {
        if (currentUserId is null)
        {
            throw ServiceException.Unauthorized();
        }

        var user = await db.Users.FindAsync(currentUserId.Value);
        if (user is null)
        {
            throw ServiceException.NotFound("User not found.");
        }

        return user.ToResponse();
    }

    public async Task<PagedUsersResponse> GetUsersAsync(int page, int pageSize, string? search, string? role)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleFilter = role.Trim().ToLower();
            query = query.Where(u => u.Role.ToLower() == roleFilter);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderBy(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => u.ToResponse())
            .ToListAsync();

        return new PagedUsersResponse(totalCount, page, pageSize, totalPages, items);
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            throw ServiceException.NotFound($"User with ID '{id}' not found.");
        }

        return user.ToResponse();
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        // TODO: Change for fluent validation
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
        {
            throw ServiceException.BadRequest("First name, last name, username, email, and password are required.");
        }

        var role = string.IsNullOrWhiteSpace(request.Role)
            ? Roles.User
            : Roles.Normalize(request.Role.Trim());

        if (!Roles.IsValid(role))
        {
            throw ServiceException.BadRequest(
                $"Invalid role '{request.Role}'. Allowed roles: {string.Join(", ", Roles.All)}.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
        {
            throw ServiceException.Conflict($"Email '{normalizedEmail}' is already registered.");
        }

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            Password = Argon2.Hash(request.Password),
            Role = role
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user.ToResponse();
    }

    public async Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            throw ServiceException.NotFound($"User with ID '{id}' not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email.Trim().ToLowerInvariant() != user.Email.ToLowerInvariant())
        {
            var trimmedEmail = request.Email.Trim().ToLowerInvariant();
            var emailExists = await db.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == trimmedEmail);
            if (emailExists)
            {
                throw ServiceException.Conflict($"Email '{trimmedEmail}' is already registered.");
            }
            user.Email = trimmedEmail;
        }

        // TODO: Use fluent validation here
        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            user.FirstName = request.FirstName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            user.LastName = request.LastName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var normalizedRole = Roles.Normalize(request.Role.Trim());
            if (!Roles.IsValid(normalizedRole))
            {
                throw ServiceException.BadRequest(
                    $"Invalid role '{request.Role}'. Allowed roles: {string.Join(", ", Roles.All)}.");
            }
            user.Role = normalizedRole;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.Password = Argon2.Hash(request.Password);
        }

        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return user.ToResponse();
    }

    public async Task DeleteUserAsync(Guid id, Guid? currentUserId)
    {
        if (currentUserId == id)
        {
            throw ServiceException.BadRequest("Administrators cannot delete their own account.");
        }

        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            throw ServiceException.NotFound($"User with ID '{id}' not found.");
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }
}
