using BitronCore.Application.Common;
using BitronCore.Application.DTOs;
using BitronCore.Domain.Entities;

namespace BitronCore.Application.Interfaces;

public interface IInspeccionRepository
{
    Task<bool> ExisteAsync(Guid transaccionId, CancellationToken ct = default);
    Task AddAsync(Inspeccion inspeccion, CancellationToken ct = default);
    Task<Inspeccion?> GetByIdAsync(Guid transaccionId, CancellationToken ct = default);
    Task<PagedResult<Inspeccion>> ListAsync(InspeccionFiltroDto filtro, CancellationToken ct = default);
    Task<EstadisticasDto> GetEstadisticasAsync(EstadisticasFiltroDto filtro, CancellationToken ct = default);
    Task<IEnumerable<DispositivoDto>> GetDispositivosAsync(CancellationToken ct = default);
}
