using AutoMapper;
using InfarlabSis.Application.DTOs.Devoluciones;
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
    public class DetalleDevolucionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DetalleDevolucionController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetDetalleDevoluciones()
        {
            var detallesEntidad = await _context.DetalleDevoluciones
                .Where(d => d.Estado)
                .Include(d => d.Devolucion)
                .Include(d => d.Producto)
                .ToListAsync();

            var detallesDto = _mapper.Map<List<DetalleDevolucionDto>>(detallesEntidad);
            return Ok(detallesDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetalleDevolucion(int id)
        {
            var detalleEntidad = await _context.DetalleDevoluciones
                .Include(d => d.Devolucion)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(d => d.IdDetalleDevolucion == id);

            if (detalleEntidad == null)
            {
                return NotFound(new { mensaje = "Detalle de devolución no encontrado." });
            }
                
            var detalleDto = _mapper.Map<DetalleDevolucionDto>(detalleEntidad);
            return Ok(detalleDto);
        }

        [HttpPost]
        public async Task<IActionResult> CrearDetalleDevolucion([FromBody] DetalleDevolucionCreaDto dto)
        {
            // Validar que la devolución existe
            var devolucionExiste = await _context.Devoluciones.AnyAsync(d => d.IdDevolucion == dto.IdDevolucion);
            if (!devolucionExiste)
                return BadRequest(new { mensaje = "La devolución no existe." });

            // Validar que el producto existe
            var productoExiste = await _context.Productos.AnyAsync(p => p.IdProducto == dto.IdProducto);
            if (!productoExiste)
                return BadRequest(new { mensaje = "El producto no existe." });

            var detalle = new DetalleDevolucion
            {
                IdDevolucion = dto.IdDevolucion, // ← Asegúrate de que esto NO sea 0
                IdProducto = dto.IdProducto,
                Cantidad = dto.Cantidad,
                Estado = true
            };

            _context.DetalleDevoluciones.Add(detalle);
            await _context.SaveChangesAsync();

            // Retornar el detalle creado con su ID real
            var detalleDto = _mapper.Map<DetalleDevolucionDto>(detalle);
            return CreatedAtAction(nameof(GetDetalleDevolucion), new { id = detalle.IdDetalleDevolucion }, detalleDto);
        }

    }
}