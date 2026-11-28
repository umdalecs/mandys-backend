using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IKitchenOrderNotifier
{
    Task NotifyOrderCreatedAsync(OrderResponse order);

    Task NotifyOrderCompletedAsync(OrderResponse order);

    Task NotifyOrderUncompletedAsync(OrderResponse order);
}
