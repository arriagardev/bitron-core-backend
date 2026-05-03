namespace BitronCore.Application.DTOs;

public class DispositivoDto
{
    public string DispositivoId { get; init; } = default!;
    public string Linea { get; init; } = default!;
    public DateTime UltimaConexion { get; init; }
    public int TotalInspecciones { get; init; }
}
