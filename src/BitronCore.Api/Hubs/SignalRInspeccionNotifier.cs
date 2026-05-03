using BitronCore.Application.DTOs;
using BitronCore.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BitronCore.Api.Hubs;

public class SignalRInspeccionNotifier : IInspeccionNotifier
{
    private readonly IHubContext<InspeccionHub> _hubContext;

    public SignalRInspeccionNotifier(IHubContext<InspeccionHub> hubContext)
        => _hubContext = hubContext;

    public async Task NotificarAsync(InspeccionResponseDto dto, string linea, CancellationToken ct = default)
    {
        await _hubContext.Clients.Group($"linea_{linea}").SendAsync("NuevaInspeccion", dto, ct);
        await _hubContext.Clients.Group("todas").SendAsync("NuevaInspeccion", dto, ct);
    }
}
