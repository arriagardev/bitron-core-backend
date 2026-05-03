using BitronCore.Application.DTOs;
using BitronCore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BitronCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadisticasController : ControllerBase
{
    private readonly InspeccionService _service;

    public EstadisticasController(InspeccionService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(EstadisticasDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] string? linea = null,
        [FromQuery] string? dispositivoId = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        CancellationToken ct = default)
    {
        var filtro = new EstadisticasFiltroDto
        {
            Linea = linea,
            DispositivoId = dispositivoId,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta
        };

        return Ok(await _service.GetEstadisticasAsync(filtro, ct));
    }
}
