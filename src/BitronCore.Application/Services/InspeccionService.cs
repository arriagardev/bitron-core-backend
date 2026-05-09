using BitronCore.Application.Common;
using BitronCore.Application.DTOs;
using BitronCore.Application.Interfaces;
using BitronCore.Domain.Entities;

namespace BitronCore.Application.Services;

public class InspeccionService
{
    private readonly IInspeccionRepository _repo;

    public InspeccionService(IInspeccionRepository repo) => _repo = repo;

    public async Task<(bool esNueva, InspeccionResponseDto dto)> ProcesarAsync(
        InspeccionMqttDto mqttDto,
        string linea,
        CancellationToken ct = default)
    {
        if (await _repo.ExisteAsync(mqttDto.TransaccionId, ct))
            return (false, MapToResponse(MapToEntity(mqttDto, linea)));

        var entidad = MapToEntity(mqttDto, linea);
        await _repo.AddAsync(entidad, ct);

        return (true, MapToResponse(entidad));
    }

    public Task<InspeccionResponseDto?> GetByIdAsync(Guid transaccionId, CancellationToken ct = default)
        => _repo.GetByIdAsync(transaccionId, ct)
               .ContinueWith(t => t.Result is { } e ? MapToResponse(e) : (InspeccionResponseDto?)null, ct);

    public async Task<PagedResult<InspeccionResponseDto>> ListAsync(InspeccionFiltroDto filtro, CancellationToken ct = default)
    {
        var resultado = await _repo.ListAsync(filtro, ct);
        return new PagedResult<InspeccionResponseDto>
        {
            Items = resultado.Items.Select(MapToResponse),
            TotalCount = resultado.TotalCount,
            Page = resultado.Page,
            PageSize = resultado.PageSize
        };
    }

    public Task<EstadisticasDto> GetEstadisticasAsync(EstadisticasFiltroDto filtro, CancellationToken ct = default)
        => _repo.GetEstadisticasAsync(filtro, ct);

    public Task<IEnumerable<DispositivoDto>> GetDispositivosAsync(CancellationToken ct = default)
        => _repo.GetDispositivosAsync(ct);

    private static Inspeccion MapToEntity(InspeccionMqttDto dto, string linea) => new()
    {
        TransaccionId = dto.TransaccionId,
        DispositivoId = dto.DispositivoId,
        Linea = linea,
        Timestamp = dto.Timestamp,
        ModeloDetectado = dto.Metadatos.ModeloDetectado,
        TiempoTotalMs = dto.Metadatos.TiempoTotalMs,
        VersionModeloKnn = dto.Metadatos.VersionModeloKnn,
        VeredictoGlobal = dto.VeredictoGlobal,
        EvidenciaUrl = dto.EvidenciaUrl,
        CreadoEn = DateTime.UtcNow,
        AnalisisRois = dto.AnalisisRoi.Select(r => new AnalisisRoi
        {
            Zona = r.Zona,
            Densidad = r.Densidad,
            BrilloPromedio = r.BrilloPromedio,
            EsOk = r.EsOk
        }).ToList()
    };

    private static InspeccionResponseDto MapToResponse(Inspeccion e) => new()
    {
        TransaccionId = e.TransaccionId,
        DispositivoId = e.DispositivoId,
        Linea = e.Linea,
        Timestamp = e.Timestamp,
        ModeloDetectado = e.ModeloDetectado,
        TiempoTotalMs = e.TiempoTotalMs,
        VersionModeloKnn = e.VersionModeloKnn,
        VeredictoGlobal = e.VeredictoGlobal,
        EvidenciaUrl = e.EvidenciaUrl,
        CreadoEn = e.CreadoEn,
        AnalisisRois = e.AnalisisRois.Select(r => new AnalisisRoiResponseDto
        {
            Zona = r.Zona,
            Densidad = r.Densidad,
            BrilloPromedio = r.BrilloPromedio,
            EsOk = r.EsOk
        }).ToList()
    };
}
