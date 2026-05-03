using BitronCore.Application.DTOs;

namespace BitronCore.Application.Interfaces;

public interface IInspeccionNotifier
{
    Task NotificarAsync(InspeccionResponseDto dto, string linea, CancellationToken ct = default);
}
