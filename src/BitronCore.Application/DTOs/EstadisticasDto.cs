namespace BitronCore.Application.DTOs;

public class EstadisticasDto
{
    public int Total { get; init; }
    public int OkCount { get; init; }
    public int NgCount { get; init; }
    public double YieldRate { get; init; }
    public double PromTiempoMs { get; init; }
    public string? Linea { get; init; }
    public string? DispositivoId { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
}
