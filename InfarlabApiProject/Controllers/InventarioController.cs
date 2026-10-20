using InfarlabSis.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfarlabApiProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public InventarioController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        [HttpGet("stock")]
        public async Task<IActionResult> ObtenerStock(
            [FromQuery] int? idAlmacen,
            [FromQuery] int? idProducto,
            CancellationToken cancellationToken)
        {
            var stock = await _inventarioService.ObtenerStockPorAlmacenAsync(idAlmacen, idProducto, cancellationToken);
            return Ok(stock);
        }

        [HttpGet("kardex")]
        public async Task<IActionResult> ObtenerKardex(
            [FromQuery] int? idProducto,
            [FromQuery] int? idAlmacen,
            [FromQuery] int? idLote,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int limite = 200,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var historial = await _inventarioService.ObtenerHistorialKardexAsync(
                    idProducto, idAlmacen, idLote, desde, hasta, limite, cancellationToken);
                return Ok(historial);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
