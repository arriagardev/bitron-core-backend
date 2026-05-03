using BitronCore.Application.DTOs;
using BitronCore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BitronCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InspeccionesController : ControllerBase
{
    private readonly InspeccionService _service;

    public InspeccionesController(InspeccionService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? veredicto = null,
        [FromQuery] string? dispositivoId = null,
        [FromQuery] string? linea = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        CancellationToken ct = default)
    {
        var filtro = new InspeccionFiltroDto
        {
            Page = Math.Max(1, page),
            PageSize = Math.Clamp(pageSize, 1, 100),
            Veredicto = veredicto,
            DispositivoId = dispositivoId,
            Linea = linea,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta
        };

        var resultado = await _service.ListAsync(filtro, ct);
        return Ok(resultado);
    }

    [HttpGet("{transaccionId:guid}")]
    [ProducesResponseType(typeof(InspeccionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid transaccionId, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(transaccionId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}
