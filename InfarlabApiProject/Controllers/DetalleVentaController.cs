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
    [Route("api/v1/[controller]")]
    public class DetalleVentaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DetalleVentaController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetDetalleVentas()
        {
            var detallesEntidad = await _context.DetalleVentas
                .Where(d => d.Estado)
                .Include(d => d.Venta)
                .Include(d => d.Producto)
                .ToListAsync();

            var detallesDto = _mapper.Map<List<DetalleVentaDto>>(detallesEntidad);
            return Ok(detallesDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetalleVenta(int id)
        {
            var detalleEntidad = await _context.DetalleVentas
                .Include(d => d.Venta)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(d => d.IdDetalleVenta == id);

            if (detalleEntidad == null)
                return NotFound(new { mensaje = "Detalle de venta no encontrado." });

            var detalleDto = _mapper.Map<DetalleVentaDto>(detalleEntidad);
            return Ok(detalleDto);
        }

        [HttpPost]
        public async Task<IActionResult> CrearDetalleVenta([FromBody] DetalleVentaCreaDto dto)
        {
            var nuevoDetalle = _mapper.Map<DetalleVenta>(dto);
            nuevoDetalle.Estado = true;
            nuevoDetalle.Subtotal = dto.Cantidad * dto.PrecioUnitario;

            _context.DetalleVentas.Add(nuevoDetalle);

            var ventaPadre = await _context.Ventas.FindAsync(dto.IdVenta);
            if (ventaPadre != null)
            {
                ventaPadre.Total += nuevoDetalle.Subtotal;
                _context.Ventas.Update(ventaPadre);
            }

            await _context.SaveChangesAsync();

            var resultadoDto = _mapper.Map<DetalleVentaDto>(nuevoDetalle);
            return CreatedAtAction(nameof(GetDetalleVenta), new { id = nuevoDetalle.IdDetalleVenta }, resultadoDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarDetalleVenta(int id, [FromBody] DetalleVentaDto dto)
        {
            var detalleExistente = await _context.DetalleVentas.FindAsync(id);

            if (detalleExistente == null)
                return NotFound(new { mensaje = "Detalle de venta no encontrado." });

            var diferencia = (dto.Cantidad * dto.PrecioUnitario) - detalleExistente.Subtotal;

            _mapper.Map(dto, detalleExistente);
            detalleExistente.Subtotal = dto.Cantidad * dto.PrecioUnitario;

            var ventaPadre = await _context.Ventas.FindAsync(detalleExistente.IdVenta);
            if (ventaPadre != null)
            {
                ventaPadre.Total += diferencia;
                _context.Ventas.Update(ventaPadre);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarDetalleVenta(int id)
        {
            var detalle = await _context.DetalleVentas.FindAsync(id);

            if (detalle == null)
                return NotFound(new { mensaje = "Detalle de venta no encontrado." });

            detalle.Estado = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Detalle de venta desactivado exitosamente." });
        }
    }
}