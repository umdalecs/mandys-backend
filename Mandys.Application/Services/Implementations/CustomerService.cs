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

        var existing = await users.FindByEmailAsync(email);
        if (existing is not null)
        {
            // Same message either way, so a caller cannot learn from the
            // response that an address is on file as staff.
            if (existing.HasLogin ||
                !string.Equals(existing.Role, Roles.Customer, StringComparison.OrdinalIgnoreCase))
            {
                throw ServiceException.Conflict($"Email '{email}' is already registered.");
            }

            existing.UpdateProfile(firstName, lastName);
            existing.SetPasswordHash(passwordHash);

            await users.UpdateAsync(existing);

            return existing.ToCustomerResponse();
        }

        var created = await users.AddAsync(new User(
            0,
            firstName,
            lastName,
            email,
            passwordHash,
            Roles.Customer));

        return created.ToCustomerResponse();
    }

    public async Task<CustomerResponse> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var customer = await GetCustomerAsync(id);
        if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
        {
            customer.UpdateProfile(
                string.IsNullOrWhiteSpace(request.FirstName) ? customer.FirstName : request.FirstName.Trim(),
                string.IsNullOrWhiteSpace(request.LastName) ? customer.LastName : request.LastName.Trim());
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();
            if (!string.Equals(email, customer.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await users.ExistsByEmailAsync(email, id))
                {
                    throw ServiceException.Conflict($"Email '{email}' is already registered.");
                }

                customer.ChangeEmail(email);
            }
        }

        await users.UpdateAsync(customer);

        return customer.ToCustomerResponse();
    }

    private async Task<User> GetCustomerAsync(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null || !Roles.IsValid(user.Role) ||
            !string.Equals(user.Role, Roles.Customer, StringComparison.OrdinalIgnoreCase))
        {
            throw ServiceException.NotFound($"Customer with ID '{id}' not found.");
        }

        return user;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null)
        {
            throw ServiceException.NotFound($"User with ID '{id}' not found.");
        }

        await users.RemoveAsync(id);
        await refreshTokens.RevokeAllAsync(id);
    }
}
