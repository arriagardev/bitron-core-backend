using System.Text;
using System.Text.Json;
using BitronCore.Application.DTOs;
using BitronCore.Application.Interfaces;
using BitronCore.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace BitronCore.Infrastructure.Mqtt;

public class MqttBackgroundService : BackgroundService
{
    private readonly MqttOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MqttBackgroundService> _logger;
    private IManagedMqttClient? _client;

    public MqttBackgroundService(
        IOptions<MqttOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<MqttBackgroundService> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client = new MqttFactory().CreateManagedMqttClient();

        _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;

        _client.ConnectedAsync += _ =>
        {
            _logger.LogInformation("MQTT conectado a {Host}:{Port}", _options.Host, _options.Port);
            return Task.CompletedTask;
        };

        _client.DisconnectedAsync += _ =>
        {
            _logger.LogWarning("MQTT desconectado. Reconectando en {Delay}s...", _options.ReconnectDelaySeconds);
            return Task.CompletedTask;
        };

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.Host, _options.Port)
            .WithClientId(_options.ClientId)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(_options.KeepAliveSeconds))
            .WithCleanSession(false)
            .Build();

        var managedOptions = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(_options.ReconnectDelaySeconds))
            .WithClientOptions(mqttClientOptions)
            .Build();

        await _client.StartAsync(managedOptions);
        await _client.SubscribeAsync(_options.TopicPattern);

        _logger.LogInformation("Suscrito al tópico: {Topic}", _options.TopicPattern);

        await Task.Delay(Timeout.Infinite, stoppingToken);

        await _client.StopAsync();
    }

    private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        var topic = args.ApplicationMessage.Topic;
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);

        InspeccionMqttDto mqttDto;
        try
        {
            mqttDto = JsonSerializer.Deserialize<InspeccionMqttDto>(payload)
                      ?? throw new InvalidOperationException("Payload vacío");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payload inválido en tópico {Topic}", topic);
            return;
        }

        // Extraer nombre de línea del tópico: bitron/{linea}/inspeccion/resultado
        var linea = topic.Split('/').ElementAtOrDefault(1) ?? "desconocida";

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<InspeccionService>();
        var notifier = scope.ServiceProvider.GetRequiredService<IInspeccionNotifier>();

        try
        {
            var (esNueva, dto) = await service.ProcesarAsync(mqttDto, linea);

            if (esNueva)
                await notifier.NotificarAsync(dto, linea);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando inspección {Id}", mqttDto.TransaccionId);
        }
    }
}
