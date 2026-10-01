using Mandys.DTOs;

namespace Mandys.Services.Interfaces;

public interface ICustomerService
{
    Task<PagedCustomersResponse> GetCustomersAsync(int page, int pageSize, string? search, string? orderBy);

    Task<CustomerResponse> GetByIdAsync(int id);

    Task<CustomerResponse> RegisterAsync(RegisterCustomerRequest request);

    Task<CustomerResponse> UpdateAsync(int id, UpdateCustomerRequest request);

    Task DeleteAsync(int id);
}
