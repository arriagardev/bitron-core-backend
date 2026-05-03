namespace BitronCore.Application.DTOs;

public class InspeccionFiltroDto
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Veredicto { get; init; }
    public string? DispositivoId { get; init; }
    public string? Linea { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
}

public class EstadisticasFiltroDto
{
    public string? Linea { get; init; }
    public string? DispositivoId { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
}
