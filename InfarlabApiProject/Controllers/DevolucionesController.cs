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
    [Route("api/[controller]")]
    public class DevolucionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DevolucionesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetDevoluciones()
        {
            var devolucionesEntidad = await _context.Devoluciones
                .Where(d => d.Estado)
                .Include(d => d.Venta)
                .Include(d => d.Cliente)
                .ToListAsync();

            var devolucionesDto = _mapper.Map<List<DevolucionDto>>(devolucionesEntidad);
            return Ok(devolucionesDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDevolucion(int id)
        {
            var devolucionEntidad = await _context.Devoluciones
                .Include(d => d.DetallesDevolucion) 
                    .ThenInclude(dd => dd.Producto)
                .Include(d => d.Venta)
                .Include(d => d.Cliente)
                .FirstOrDefaultAsync(d => d.IdDevolucion == id);

            if (devolucionEntidad == null)
            {
                return NotFound(new { mensaje = "Devolución no encontrada." });
            }
           
            var devolucionDto = _mapper.Map<DevolucionConDetallesDto>(devolucionEntidad);
            return Ok(devolucionDto);
        }

        [HttpPost]
        public async Task<IActionResult> CrearDevolucion([FromBody] DevolucionCreaDto dto)
        {
            var nuevaDevolucion = new Devolucion
            {
                FechaDevolucion = dto.FechaDevolucion,
                MotivoDevolucion = dto.MotivoDevolucion,
                IdVenta = dto.IdVenta,
                IdCliente = dto.IdCliente,
                Estado = true
            };

            _context.Devoluciones.Add(nuevaDevolucion); 
            await _context.SaveChangesAsync();

            foreach (var detalleDto in dto.Detalles)
            {
                var nuevoDetalle = new DetalleDevolucion
                {
                    IdDevolucion = nuevaDevolucion.IdDevolucion,
                    IdProducto = detalleDto.IdProducto,
                    Cantidad = detalleDto.Cantidad,
                    Estado = true
                };
                _context.DetalleDevoluciones.Add(nuevoDetalle);
            }

            await _context.SaveChangesAsync();

            var resultadoDto = _mapper.Map<DevolucionDto>(nuevaDevolucion);
            return CreatedAtAction(nameof(GetDevolucion), new { id = nuevaDevolucion.IdDevolucion }, resultadoDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarDevolucion(int id, [FromBody] DevolucionDto dto)
        {
            var devolucionExistente = await _context.Devoluciones.FindAsync(id); 

            if (devolucionExistente == null)
            {
                return NotFound(new { mensaje = "Devolución no encontrada." });
            }
                
            _mapper.Map(dto, devolucionExistente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarDevolucion(int id)
        {
            var devolucion = await _context.Devoluciones  
                .Include(d => d.DetallesDevolucion)  
                .FirstOrDefaultAsync(d => d.IdDevolucion == id);

            if (devolucion == null)
            {
                return NotFound(new { mensaje = "Devolución no encontrada." });
            }    
                
            devolucion.Estado = false;
            foreach (var detalle in devolucion.DetallesDevolucion)
            {
                detalle.Estado = false;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Devolución desactivada exitosamente." });
        }
    }
}