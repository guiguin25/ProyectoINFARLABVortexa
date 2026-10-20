using AutoMapper;
using InfarlabSis.Application.DTOs.Devoluciones;
using InfarlabSis.Application.Interfaces;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfarlabApiProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DevolucionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDevolucionService _devolucionService;

        public DevolucionesController(ApplicationDbContext context, IMapper mapper, IDevolucionService devolucionService)
        {
            _context = context;
            _mapper = mapper;
            _devolucionService = devolucionService;
        }

        [HttpPost("reproceso")]
        public async Task<IActionResult> RegistrarDevolucionReproceso(
            [FromBody] DevolucionReprocesoDto dto,
            CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idUsuario))
                return Unauthorized(new { mensaje = "El token no contiene un identificador de usuario válido." });

            dto.IdUsuario = idUsuario;
            try
            {
                var resultado = await _devolucionService.RegistrarDevolucionConReprocesoAsync(dto, cancellationToken);
                return Created($"/api/v1/devoluciones/{resultado.IdDevolucion}", resultado);
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

    }
}