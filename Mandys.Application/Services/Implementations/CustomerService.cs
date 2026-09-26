using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class CustomerService(
    IUserRepository users,
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

    /// <summary>
    /// Self-registration, and the way a counter customer claims the account
    /// the counter already created for them: if the email is on file with no
    /// login, the credentials are attached to that row and its name is
    /// updated. An email that already has a login, or that belongs to anyone
    /// other than a customer, is a conflict.
    /// The role is forced to customer and the branch is left empty, so a
    /// self-registered row can never hold staff privileges; the domain
    /// already allows a customer without a branch.
    /// </summary>
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

    /// <summary>
    /// Counter creation without login credentials: null password hash, so the
    /// row can never log in. An email is kept when given, because that is
    /// how the customer is found again to reclaim stored points. The role is
    /// forced to customer and the branch is left empty.
    /// </summary>
    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
    {
        var email = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim().ToLowerInvariant();

        if (email is not null && await users.ExistsByEmailAsync(email))
        {
            throw ServiceException.Conflict($"Email '{email}' is already registered.");
        }

        var created = await users.AddAsync(new User(
            0,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            email,
            null,
            Roles.Customer));

        return created.ToCustomerResponse();
    }

    /// <summary>
    /// Counter-side edits. Names and email only: the role, the branch and
    /// the password stay under the user module, and an id that is not a
    /// customer is rejected so this endpoint cannot edit staff.
    /// </summary>
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

    public async Task<CustomerResponse> BanAsync(int id)
    {
        var customer = await GetCustomerAsync(id);
        customer.Ban();

        await users.UpdateAsync(customer);

        return customer.ToCustomerResponse();
    }

    public async Task<CustomerResponse> UnbanAsync(int id)
    {
        var customer = await GetCustomerAsync(id);
        customer.Unban();

        await users.UpdateAsync(customer);

        return customer.ToCustomerResponse();
    }

    /// <summary>
    /// Loads a customer, treating any other role as not found so this module
    /// can never act on a staff account.
    /// </summary>
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
}
