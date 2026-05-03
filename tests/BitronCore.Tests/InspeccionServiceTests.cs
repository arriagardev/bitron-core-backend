using BitronCore.Application.DTOs;
using BitronCore.Application.Interfaces;
using BitronCore.Application.Services;
using BitronCore.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BitronCore.Tests;

public class InspeccionServiceTests
{
    private readonly Mock<IInspeccionRepository> _repoMock = new();
    private readonly Mock<ILogger<InspeccionService>> _loggerMock = new();

    private InspeccionService CreateService() =>
        new(_repoMock.Object, _loggerMock.Object);

    [Fact]
    public async Task ProcesarAsync_GuardaNuevoRegistro_CuandoNoExisteDuplicado()
    {
        var dto = BuildMqttDto();
        _repoMock.Setup(r => r.ExisteAsync(dto.TransaccionId, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Inspeccion>(), default)).Returns(Task.CompletedTask);

        var service = CreateService();
        var (esNueva, response) = await service.ProcesarAsync(dto, "linea1");

        esNueva.Should().BeTrue();
        response.TransaccionId.Should().Be(dto.TransaccionId);
        response.Linea.Should().Be("linea1");
        response.VeredictoGlobal.Should().Be("NG");
        response.AnalisisRois.Should().HaveCount(3);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Inspeccion>(), default), Times.Once);
    }

    [Fact]
    public async Task ProcesarAsync_IgnoraDuplicado_CuandoYaExiste()
    {
        var dto = BuildMqttDto();
        _repoMock.Setup(r => r.ExisteAsync(dto.TransaccionId, default)).ReturnsAsync(true);

        var service = CreateService();
        var (esNueva, _) = await service.ProcesarAsync(dto, "linea1");

        esNueva.Should().BeFalse();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Inspeccion>(), default), Times.Never);
    }

    [Fact]
    public async Task ProcesarAsync_MapaEvidenciaUrl_SoloParaNG()
    {
        var dto = BuildMqttDto(veredicto: "NG", evidenciaUrl: "http://10.0.0.5/auditoria/img.jpg");
        _repoMock.Setup(r => r.ExisteAsync(dto.TransaccionId, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Inspeccion>(), default)).Returns(Task.CompletedTask);

        var service = CreateService();
        var (_, response) = await service.ProcesarAsync(dto, "linea1");

        response.EvidenciaUrl.Should().Be("http://10.0.0.5/auditoria/img.jpg");
    }

    private static InspeccionMqttDto BuildMqttDto(
        string veredicto = "NG",
        string? evidenciaUrl = "http://10.0.0.5/img.jpg") => new()
    {
        TransaccionId = Guid.NewGuid(),
        DispositivoId = "rasp_ensamble_L1",
        Timestamp = DateTime.UtcNow,
        Metadatos = new MetadatosMqttDto
        {
            ModeloDetectado = "A",
            TiempoTotalMs = 435,
            VersionModeloKnn = "v1.2.0"
        },
        VeredictoGlobal = veredicto,
        EvidenciaUrl = evidenciaUrl,
        AnalisisRoi = new List<AnalisisRoiMqttDto>
        {
            new() { Zona = "Asiento", Densidad = 85.2, BrilloPromedio = 180.5, EsOk = true },
            new() { Zona = "SET", Densidad = 88.1, BrilloPromedio = 190.0, EsOk = true },
            new() { Zona = "Boton 1", Densidad = 12.5, BrilloPromedio = 45.2, EsOk = false }
        }
    };
}
