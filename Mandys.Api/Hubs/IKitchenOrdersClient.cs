using Mandys.Application.DTOs;

namespace Mandys.Api.Hubs;

public interface IKitchenOrdersClient
{
    Task OrderCreated(OrderResponse order);

    Task OrderCompleted(OrderResponse order);

    Task OrderUncompleted(OrderResponse order);
}
