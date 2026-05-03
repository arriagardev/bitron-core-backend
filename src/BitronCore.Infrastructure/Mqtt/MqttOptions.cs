namespace BitronCore.Infrastructure.Mqtt;

public class MqttOptions
{
    public const string SectionName = "Mqtt";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1883;
    public string ClientId { get; set; } = "bitron-core-backend";
    public string TopicPattern { get; set; } = "bitron/+/inspeccion/resultado";
    public int KeepAliveSeconds { get; set; } = 60;
    public int ReconnectDelaySeconds { get; set; } = 5;
}
