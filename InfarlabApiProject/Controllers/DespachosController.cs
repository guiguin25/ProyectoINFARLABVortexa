using System.Security.Claims;
using InfarlabSis.Application.DTOs.Despachos;
using InfarlabSis.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfarlabApiProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DespachosController : ControllerBase
    {
        private readonly IDespachoService _despachoService;

        public DespachosController(IDespachoService despachoService)
        {
            _despachoService = despachoService;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarEntrega(
            [FromBody] CrearEntregaDespachoDto dto,
            CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idUsuario))
                return Unauthorized(new { mensaje = "El token no contiene un identificador de usuario válido." });

            dto.IdUsuario = idUsuario;
            try
            {
                var entrega = await _despachoService.RegistrarEntregaParcialAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(ObtenerEntregasPorVenta), new { idVenta = dto.IdVenta }, entrega);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("venta/{idVenta:int}")]
        public async Task<IActionResult> ObtenerEntregasPorVenta(int idVenta, CancellationToken cancellationToken)
        {
            var entregas = await _despachoService.ObtenerEntregasPorVentaAsync(idVenta, cancellationToken);
            return Ok(entregas);
        }
    }
}
