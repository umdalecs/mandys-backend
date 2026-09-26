using Mandys.DTOs;

namespace Mandys.Services.Interfaces;

/// <summary>
/// Customer self-service and counter management, kept apart from the
/// staff-facing user module.
/// </summary>
public interface ICustomerService
{
    Task<PagedCustomersResponse> GetCustomersAsync(int page, int pageSize, string? search);

    /// <summary>
    /// Registers a customer account with login credentials. The role is
    /// always <see cref="Domain.Roles.Customer"/> and the branch is never
    /// set, regardless of what the caller sends.
    /// </summary>
    Task<CustomerResponse> RegisterAsync(RegisterCustomerRequest request);

    /// <summary>
    /// Creates a customer that has no login credentials: null password hash,
    /// so the row can never log in. Credentials can be added later through
    /// the user module.
    /// </summary>
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);

    /// <summary>
    /// Edits the counter details of a customer. Rejects ids that belong to
    /// anyone other than a customer, so this cannot be used to edit staff.
    /// </summary>
    Task<CustomerResponse> UpdateAsync(int id, UpdateCustomerRequest request);

    /// <summary>
    /// Bans a customer: it can no longer log in and its sessions are cut.
    /// Idempotent, so banning twice keeps the original timestamp.
    /// </summary>
    Task<CustomerResponse> BanAsync(int id);

    /// <summary>Lifts a ban. Idempotent.</summary>
    Task<CustomerResponse> UnbanAsync(int id);
}
