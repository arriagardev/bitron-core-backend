using Microsoft.AspNetCore.SignalR;

namespace BitronCore.Api.Hubs;

public class InspeccionHub : Hub
{
    public async Task JoinLinea(string linea)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"linea_{linea}");

    public async Task LeaveLinea(string linea)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"linea_{linea}");

    public async Task JoinTodas()
        => await Groups.AddToGroupAsync(Context.ConnectionId, "todas");
}
