using Mandys.Application.Common;
using Mandys.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Mandys.Api.Hubs;

[Authorize(Roles = Roles.KitchenChief)]
public class KitchenOrdersHub : Hub<IKitchenOrdersClient>
{
    public static string BranchGroup(int branchId) => $"branch:{branchId}";

    public override async Task OnConnectedAsync()
    {
        var branchId = Context.User?.GetBranchId();
        if (!branchId.HasValue)
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, BranchGroup(branchId.Value));
        await base.OnConnectedAsync();
    }
}
