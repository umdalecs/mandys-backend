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
        string? status,
        int page,
        int pageSize,
        string? orderBy);

    Task<OrderResponse> GetOrderByIdAsync(
        int? branchId,
        int orderId,
        int userId,
        string userRole,
        int? userBranchId);

    Task<OrderResponse> MarkOrderCompletedAsync(
        int orderId,
        string userRole,
        int? userBranchId);

    Task<OrderResponse> MarkOrderUncompletedAsync(
        int orderId,
        string userRole,
        int? userBranchId);
}
