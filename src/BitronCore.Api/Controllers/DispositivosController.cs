using BitronCore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BitronCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DispositivosController : ControllerBase
{
    private readonly InspeccionService _service;

    public DispositivosController(InspeccionService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _service.GetDispositivosAsync(ct));
}
