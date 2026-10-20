using AutoMapper;
using InfarlabSis.Application.DTOs.Productos;
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
    public class AlmacenesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AlmacenesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlmacenDto>>> GetAlmacenes(CancellationToken cancellationToken)
        {
            var almacenes = await _context.Almacenes.AsNoTracking().OrderBy(a => a.Nombre).ToListAsync(cancellationToken);
            return Ok(_mapper.Map<List<AlmacenDto>>(almacenes));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AlmacenDto>> GetAlmacen(int id, CancellationToken cancellationToken)
        {
            var almacen = await _context.Almacenes.AsNoTracking().FirstOrDefaultAsync(a => a.IdAlmacen == id, cancellationToken);
            return almacen is null ? NotFound(new { mensaje = "Almacén no encontrado." }) : Ok(_mapper.Map<AlmacenDto>(almacen));
        }

        [HttpPost]
        public async Task<ActionResult<AlmacenDto>> CrearAlmacen([FromBody] AlmacenDto dto, CancellationToken cancellationToken)
        {
            var almacen = new Almacen
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                TipoArea = dto.TipoArea.Trim(),
                Estado = dto.Estado
            };
            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync(cancellationToken);

            var resultado = _mapper.Map<AlmacenDto>(almacen);
            return CreatedAtAction(nameof(GetAlmacen), new { id = almacen.IdAlmacen }, resultado);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarAlmacen(int id, [FromBody] AlmacenDto dto, CancellationToken cancellationToken)
        {
            var almacen = await _context.Almacenes.FirstOrDefaultAsync(a => a.IdAlmacen == id, cancellationToken);
            if (almacen is null)
                return NotFound(new { mensaje = "Almacén no encontrado." });

            almacen.Nombre = dto.Nombre.Trim();
            almacen.Descripcion = dto.Descripcion?.Trim();
            almacen.TipoArea = dto.TipoArea.Trim();
            almacen.Estado = dto.Estado;
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarAlmacen(int id, CancellationToken cancellationToken)
        {
            var almacen = await _context.Almacenes.FirstOrDefaultAsync(a => a.IdAlmacen == id, cancellationToken);
            if (almacen is null)
                return NotFound(new { mensaje = "Almacén no encontrado." });

            almacen.Estado = false;
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
    }
}
