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
    public class UnidadesMedidaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UnidadesMedidaController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnidadMedidaDto>>> GetUnidades(CancellationToken cancellationToken)
        {
            var unidades = await _context.UnidadesMedida.AsNoTracking().OrderBy(u => u.Nombre).ToListAsync(cancellationToken);
            return Ok(_mapper.Map<List<UnidadMedidaDto>>(unidades));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UnidadMedidaDto>> GetUnidad(int id, CancellationToken cancellationToken)
        {
            var unidad = await _context.UnidadesMedida.AsNoTracking().FirstOrDefaultAsync(u => u.IdUnidad == id, cancellationToken);
            return unidad is null ? NotFound(new { mensaje = "Unidad de medida no encontrada." }) : Ok(_mapper.Map<UnidadMedidaDto>(unidad));
        }

        [HttpPost]
        public async Task<ActionResult<UnidadMedidaDto>> CrearUnidad([FromBody] UnidadMedidaDto dto, CancellationToken cancellationToken)
        {
            var nombre = dto.Nombre.Trim();
            if (await _context.UnidadesMedida.AnyAsync(u => u.Nombre == nombre, cancellationToken))
                return Conflict(new { mensaje = "Ya existe una unidad de medida con ese nombre." });

            var unidad = new UnidadMedida { Nombre = nombre, Estado = dto.Estado };
            _context.UnidadesMedida.Add(unidad);
            await _context.SaveChangesAsync(cancellationToken);
            var resultado = _mapper.Map<UnidadMedidaDto>(unidad);
            return CreatedAtAction(nameof(GetUnidad), new { id = unidad.IdUnidad }, resultado);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarUnidad(int id, [FromBody] UnidadMedidaDto dto, CancellationToken cancellationToken)
        {
            var unidad = await _context.UnidadesMedida.FirstOrDefaultAsync(u => u.IdUnidad == id, cancellationToken);
            if (unidad is null)
                return NotFound(new { mensaje = "Unidad de medida no encontrada." });

            var nombre = dto.Nombre.Trim();
            if (await _context.UnidadesMedida.AnyAsync(u => u.IdUnidad != id && u.Nombre == nombre, cancellationToken))
                return Conflict(new { mensaje = "Ya existe una unidad de medida con ese nombre." });

            unidad.Nombre = nombre;
            unidad.Estado = dto.Estado;
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DesactivarUnidad(int id, CancellationToken cancellationToken)
        {
            var unidad = await _context.UnidadesMedida.FirstOrDefaultAsync(u => u.IdUnidad == id, cancellationToken);
            if (unidad is null)
                return NotFound(new { mensaje = "Unidad de medida no encontrada." });

            if (await _context.Productos.AnyAsync(p => p.IdUnidad == id, cancellationToken))
                return Conflict(new { mensaje = "No se puede desactivar una unidad utilizada por productos." });

            unidad.Estado = false;
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
    }
}
