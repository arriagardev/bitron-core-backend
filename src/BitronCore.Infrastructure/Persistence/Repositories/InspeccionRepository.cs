using BitronCore.Application.Common;
using BitronCore.Application.DTOs;
using BitronCore.Application.Interfaces;
using BitronCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitronCore.Infrastructure.Persistence.Repositories;

public class InspeccionRepository : IInspeccionRepository
{
    private readonly AppDbContext _db;

    public InspeccionRepository(AppDbContext db) => _db = db;

    public Task<bool> ExisteAsync(Guid transaccionId, CancellationToken ct) =>
        _db.Inspecciones.AnyAsync(x => x.TransaccionId == transaccionId, ct);

    public async Task AddAsync(Inspeccion inspeccion, CancellationToken ct)
    {
        _db.Inspecciones.Add(inspeccion);
        await _db.SaveChangesAsync(ct);
    }

    public Task<Inspeccion?> GetByIdAsync(Guid transaccionId, CancellationToken ct) =>
        _db.Inspecciones
           .Include(x => x.AnalisisRois)
           .FirstOrDefaultAsync(x => x.TransaccionId == transaccionId, ct);

    public async Task<PagedResult<Inspeccion>> ListAsync(InspeccionFiltroDto filtro, CancellationToken ct)
    {
        var query = _db.Inspecciones.Include(x => x.AnalisisRois).AsQueryable();

        if (!string.IsNullOrEmpty(filtro.Veredicto))
            query = query.Where(x => x.VeredictoGlobal == filtro.Veredicto.ToUpper());

        if (!string.IsNullOrEmpty(filtro.DispositivoId))
            query = query.Where(x => x.DispositivoId == filtro.DispositivoId);

        if (!string.IsNullOrEmpty(filtro.Linea))
            query = query.Where(x => x.Linea == filtro.Linea);

        if (filtro.FechaDesde.HasValue)
            query = query.Where(x => x.Timestamp >= filtro.FechaDesde.Value);

        if (filtro.FechaHasta.HasValue)
            query = query.Where(x => x.Timestamp <= filtro.FechaHasta.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Inspeccion>
        {
            Items = items,
            TotalCount = total,
            Page = filtro.Page,
            PageSize = filtro.PageSize
        };
    }

    public async Task<EstadisticasDto> GetEstadisticasAsync(EstadisticasFiltroDto filtro, CancellationToken ct)
    {
        var query = _db.Inspecciones.AsQueryable();

        if (!string.IsNullOrEmpty(filtro.Linea))
            query = query.Where(x => x.Linea == filtro.Linea);

        if (!string.IsNullOrEmpty(filtro.DispositivoId))
            query = query.Where(x => x.DispositivoId == filtro.DispositivoId);

        if (filtro.FechaDesde.HasValue)
            query = query.Where(x => x.Timestamp >= filtro.FechaDesde.Value);

        if (filtro.FechaHasta.HasValue)
            query = query.Where(x => x.Timestamp <= filtro.FechaHasta.Value);

        var total = await query.CountAsync(ct);
        var okCount = await query.CountAsync(x => x.VeredictoGlobal == "OK", ct);
        var ngCount = total - okCount;
        var promTiempo = total > 0 ? await query.AverageAsync(x => (double)x.TiempoTotalMs, ct) : 0;

        return new EstadisticasDto
        {
            Total = total,
            OkCount = okCount,
            NgCount = ngCount,
            YieldRate = total > 0 ? Math.Round((double)okCount / total * 100, 2) : 0,
            PromTiempoMs = Math.Round(promTiempo, 2),
            Linea = filtro.Linea,
            DispositivoId = filtro.DispositivoId,
            FechaDesde = filtro.FechaDesde,
            FechaHasta = filtro.FechaHasta
        };
    }

    public async Task<IEnumerable<DispositivoDto>> GetDispositivosAsync(CancellationToken ct)
    {
        return await _db.Inspecciones
            .GroupBy(x => new { x.DispositivoId, x.Linea })
            .Select(g => new DispositivoDto
            {
                DispositivoId = g.Key.DispositivoId,
                Linea = g.Key.Linea,
                UltimaConexion = g.Max(x => x.Timestamp),
                TotalInspecciones = g.Count()
            })
            .OrderBy(x => x.Linea)
            .ThenBy(x => x.DispositivoId)
            .ToListAsync(ct);
    }
}
