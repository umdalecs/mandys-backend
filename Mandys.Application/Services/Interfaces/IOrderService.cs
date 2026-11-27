using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(
        int branchId,
        int userId,
        string userRole,
        int? userBranchId,
        CreateOrderRequest request);

    Task<PagedOrdersResponse> GetOrdersAsync(
        int? branchId,
        int userId,
        string userRole,
        int? userBranchId,
        string? paymentMethod,
        int page,
        int pageSize,
        string? orderBy);

    Task<OrderResponse> GetOrderByIdAsync(
        int? branchId,
        int orderId,
        int userId,
        string userRole,
        int? userBranchId);
}
