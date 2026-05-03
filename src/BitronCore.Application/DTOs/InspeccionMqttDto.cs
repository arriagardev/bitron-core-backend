using System.Text.Json.Serialization;

namespace BitronCore.Application.DTOs;

public class InspeccionMqttDto
{
    [JsonPropertyName("transaccion_id")]
    public Guid TransaccionId { get; set; }

    [JsonPropertyName("dispositivo_id")]
    public string DispositivoId { get; set; } = default!;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("metadatos")]
    public MetadatosMqttDto Metadatos { get; set; } = default!;

    [JsonPropertyName("veredicto_global")]
    public string VeredictoGlobal { get; set; } = default!;

    [JsonPropertyName("evidencia_url")]
    public string? EvidenciaUrl { get; set; }

    [JsonPropertyName("analisis_roi")]
    public List<AnalisisRoiMqttDto> AnalisisRoi { get; set; } = new();
}

public class MetadatosMqttDto
{
    [JsonPropertyName("modelo_detectado")]
    public string ModeloDetectado { get; set; } = default!;

    [JsonPropertyName("tiempo_total_ms")]
    public int TiempoTotalMs { get; set; }

    [JsonPropertyName("version_modelo_knn")]
    public string VersionModeloKnn { get; set; } = default!;
}

public class AnalisisRoiMqttDto
{
    [JsonPropertyName("zona")]
    public string Zona { get; set; } = default!;

    [JsonPropertyName("densidad")]
    public double Densidad { get; set; }

    [JsonPropertyName("brillo_promedio")]
    public double BrilloPromedio { get; set; }

    [JsonPropertyName("es_ok")]
    public bool EsOk { get; set; }
}
