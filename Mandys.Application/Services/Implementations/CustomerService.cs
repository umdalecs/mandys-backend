using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class CustomerService(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher) : ICustomerService
{
    public async Task<PagedCustomersResponse> GetCustomersAsync(int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        // The role filter is what keeps staff out of the customer list.
        var (totalCount, items) = await users.SearchAsync(search, Roles.Customer, page, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedCustomersResponse(totalCount, page, pageSize, totalPages,
            items.Select(u => u.ToCustomerResponse()).ToList());
    }

    public async Task<CustomerResponse> RegisterAsync(RegisterCustomerRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var passwordHash = passwordHasher.Hash(request.Password);

        if (await users.FindByEmailAsync(email) is not null)
            throw ServiceException.Conflict($"El correo '{email}' ya está registrado.");

        var created = await users.AddAsync(new User(
            0,
            firstName,
            lastName,
            email,
            passwordHash,
            Roles.Customer));

        return created.ToCustomerResponse();
    }

    public async Task<CustomerResponse> GetByIdAsync(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null || !string.Equals(user.Role, Roles.Customer, StringComparison.OrdinalIgnoreCase))
            throw ServiceException.NotFound($"No se encontró el cliente con ID '{id}'.");

        return user.ToCustomerResponse();
    }

    public async Task<CustomerResponse> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null || !string.Equals(user.Role, Roles.Customer, StringComparison.OrdinalIgnoreCase))
            throw ServiceException.NotFound($"No se encontró el cliente con ID '{id}'.");

        if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
        {
            user.UpdateProfile(
                string.IsNullOrWhiteSpace(request.FirstName) ? user.FirstName : request.FirstName.Trim(),
                string.IsNullOrWhiteSpace(request.LastName) ? user.LastName : request.LastName.Trim());
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();
            if (!string.Equals(email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await users.ExistsByEmailAsync(email, id))
                    throw ServiceException.Conflict($"El correo '{email}' ya está registrado.");

                user.ChangeEmail(email);
            }
        }

        await users.UpdateAsync(user);

        return user.ToCustomerResponse();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null)
            throw ServiceException.NotFound($"No se encontró el usuario con ID '{id}'.");

        await users.RemoveAsync(id);
        await refreshTokens.RevokeAllAsync(id);
    }
}
