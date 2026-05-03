namespace BitronCore.Domain.Entities;

public class Inspeccion
{
    public Guid TransaccionId { get; set; }
    public string DispositivoId { get; set; } = default!;
    public string Linea { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public string ModeloDetectado { get; set; } = default!;
    public int TiempoTotalMs { get; set; }
    public string VersionModeloKnn { get; set; } = default!;
    public string VeredictoGlobal { get; set; } = default!;
    public string? EvidenciaUrl { get; set; }
    public DateTime CreadoEn { get; set; }
    public ICollection<AnalisisRoi> AnalisisRois { get; set; } = new List<AnalisisRoi>();
}
