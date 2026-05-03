namespace BitronCore.Domain.Entities;

public class AnalisisRoi
{
    public int Id { get; set; }
    public Guid InspeccionId { get; set; }
    public string Zona { get; set; } = default!;
    public double Densidad { get; set; }
    public double BrilloPromedio { get; set; }
    public bool EsOk { get; set; }
    public Inspeccion Inspeccion { get; set; } = default!;
}
