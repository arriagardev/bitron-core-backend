namespace BitronCore.Application.DTOs;

public class InspeccionResponseDto
{
    public Guid TransaccionId { get; init; }
    public string DispositivoId { get; init; } = default!;
    public string Linea { get; init; } = default!;
    public DateTime Timestamp { get; init; }
    public string ModeloDetectado { get; init; } = default!;
    public int TiempoTotalMs { get; init; }
    public string VersionModeloKnn { get; init; } = default!;
    public string VeredictoGlobal { get; init; } = default!;
    public string? EvidenciaUrl { get; init; }
    public DateTime CreadoEn { get; init; }
    public List<AnalisisRoiResponseDto> AnalisisRois { get; init; } = new();
}

public class AnalisisRoiResponseDto
{
    public string Zona { get; init; } = default!;
    public double Densidad { get; init; }
    public double BrilloPromedio { get; init; }
    public bool EsOk { get; init; }
}
