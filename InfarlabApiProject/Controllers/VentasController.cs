using AutoMapper;
using InfarlabSis.Application.DTOs.Ventas;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfarlabApiProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VentasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetVentas()
        {
            var ventasEntidad = await _context.Ventas
                .Where(v => v.Estado)
                .Include(v => v.Usuario)
                .Include(v => v.Cliente)
                .ToListAsync();

            var ventasDto = _mapper.Map<List<VentaDto>>(ventasEntidad);
            return Ok(ventasDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVenta(int id)
        {
            var ventaEntidad = await _context.Ventas
                .Include(v => v.DetallesVenta) 
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Usuario)
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (ventaEntidad == null)
            {
                return NotFound(new { mensaje = "Venta no encontrada." });
            }    
                
            var ventaDto = _mapper.Map<VentaConDetallesDto>(ventaEntidad); 
            return Ok(ventaDto);
        }

        [HttpPost]
        public async Task<IActionResult> CrearVenta([FromBody] VentaCreaDto dto)
        {
            var nuevaVenta = new Venta
            {
                FechaVenta = dto.FechaVenta,
                TipoPago = dto.TipoPago,
                IdUsuario = dto.IdUsuario,
                IdCliente = dto.IdCliente, 
                Total = 0,
                Estado = true
            };

            _context.Ventas.Add(nuevaVenta);
            await _context.SaveChangesAsync();

            decimal totalVenta = 0;
            foreach (var detalleDto in dto.Detalles)
            {
                var subtotal = detalleDto.Cantidad * detalleDto.PrecioUnitario;
                totalVenta += subtotal;

                var nuevoDetalle = new DetalleVenta
                {
                    IdVenta = nuevaVenta.IdVenta,
                    IdProducto = detalleDto.IdProducto,
                    Cantidad = detalleDto.Cantidad,
                    PrecioUnitario = detalleDto.PrecioUnitario,
                    Subtotal = subtotal,
                    Estado = true
                };
                _context.DetalleVentas.Add(nuevoDetalle);
            }

            nuevaVenta.Total = totalVenta;
            _context.Ventas.Update(nuevaVenta);
            await _context.SaveChangesAsync();

            var resultadoDto = _mapper.Map<VentaDto>(nuevaVenta);
            return CreatedAtAction(nameof(GetVenta), new { id = nuevaVenta.IdVenta }, resultadoDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarVenta(int id, [FromBody] VentaDto dto)
        {
            var ventaExistente = await _context.Ventas.FindAsync(id);

            if (ventaExistente == null)
            {
                return NotFound(new { mensaje = "Venta no encontrada." });
            }
                
            _mapper.Map(dto, ventaExistente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.DetallesVenta)  
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound(new { mensaje = "Venta no encontrada." });
            }
                
            venta.Estado = false;
            foreach (var detalle in venta.DetallesVenta)  
            {
                detalle.Estado = false;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Venta desactivada exitosamente." });
        }
    }
}