using Mandys.Api.Hubs;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Mandys.Api.Services;

public class SignalRKitchenOrderNotifier(
    IHubContext<KitchenOrdersHub, IKitchenOrdersClient> hubContext) : IKitchenOrderNotifier
{
    public Task NotifyOrderCreatedAsync(OrderResponse order) =>
        hubContext.Clients
            .Group(KitchenOrdersHub.BranchGroup(order.BranchId))
            .OrderCreated(order);

    public Task NotifyOrderCompletedAsync(OrderResponse order) =>
        hubContext.Clients
            .Group(KitchenOrdersHub.BranchGroup(order.BranchId))
            .OrderCompleted(order);

    public Task NotifyOrderUncompletedAsync(OrderResponse order) =>
        hubContext.Clients
            .Group(KitchenOrdersHub.BranchGroup(order.BranchId))
            .OrderUncompleted(order);
}
