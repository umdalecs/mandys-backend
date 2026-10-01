using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class UserService(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher) : IUserService
{
    public async Task<UserResponse> GetCurrentUserAsync(int? currentUserId)
    {
        if (currentUserId is null)
        {
            throw ServiceException.Unauthorized();
        }

        var user = await users.GetByIdAsync(currentUserId.Value);
        if (user is null)
        {
            throw ServiceException.NotFound("Usuario no encontrado.");
        }

        return user.ToResponse();
    }
    public async Task<PagedUsersResponse> GetUsersAsync(int page, int pageSize, string? search, string? role)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await users.SearchAsync(search, role, page, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedUsersResponse(totalCount, page, pageSize, totalPages,
            items.Select(u => u.ToResponse()).ToList());
    }

    public async Task<UserResponse> GetUserByIdAsync(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null)
        {
            throw ServiceException.NotFound($"No se encontró el usuario con ID '{id}'.");
        }

        return user.ToResponse();
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        var role = string.IsNullOrWhiteSpace(request.Role)
            ? Roles.Customer
            : Roles.Normalize(request.Role.Trim());

        var email = request.Email.Trim().ToLowerInvariant();

        if (await users.ExistsByEmailAsync(email))
            throw ServiceException.Conflict($"El correo '{email}' ya está registrado.");

        var created = await users.AddAsync(new User(
            0,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            email,
            passwordHasher.Hash(request.Password),
            role,
            request.BranchId));

        return created.ToResponse();
    }

    public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null)
        {
            throw ServiceException.NotFound($"No se encontró el usuario con ID '{id}'.");
        }

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email.Trim().ToLowerInvariant() != user.Email.ToLowerInvariant())
        {
            var trimmedEmail = request.Email.Trim().ToLowerInvariant();
            if (await users.ExistsByEmailAsync(trimmedEmail, id))
            {
                throw ServiceException.Conflict($"El correo '{trimmedEmail}' ya está registrado.");
            }
            user.ChangeEmail(trimmedEmail);
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
        {
            user.UpdateProfile(
                string.IsNullOrWhiteSpace(request.FirstName) ? user.FirstName : request.FirstName.Trim(),
                string.IsNullOrWhiteSpace(request.LastName) ? user.LastName : request.LastName.Trim());
        }

        string? newRole = null;
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            newRole = Roles.Normalize(request.Role.Trim());
        }

        // Validate branch against the role taking effect, before mutating.
        var effectiveRole = newRole ?? user.Role;
        var effectiveBranch = request.ClearBranch ? null : (request.BranchId ?? user.BranchId);
        if (Roles.RequiresBranch(effectiveRole) && effectiveBranch is null)
        {
            throw ServiceException.BadRequest(
                $"El rol '{effectiveRole}' requiere una sucursal. Solo administradores y clientes pueden omitirla.");
        }


        // Branch first so a simultaneous role upgrade sees the new branch.
        if (request.ClearBranch)
        {
            user.ClearBranch();
        }
        else if (request.BranchId.HasValue)
        {
            user.SetBranch(request.BranchId);
        }

        if (newRole is not null)
        {
            user.SetRole(newRole);
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.SetPasswordHash(passwordHasher.Hash(request.Password));
        }

        await users.UpdateAsync(user);

        return user.ToResponse();
    }

    public async Task DeleteUserAsync(int id, int? currentUserId)
    {
        if (currentUserId == id)
        {
            throw ServiceException.BadRequest("Los administradores no pueden eliminar su propia cuenta.");
        }

        var user = await users.GetByIdAsync(id);
        if (user is null)
        {
            throw ServiceException.NotFound($"No se encontró el usuario con ID '{id}'.");
        }

        await users.RemoveAsync(id);
        await refreshTokens.RevokeAllAsync(id);
    }

}
