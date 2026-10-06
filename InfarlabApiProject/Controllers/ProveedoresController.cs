using AutoMapper;
using InfarlabSis.Application.DTOs.Actores;
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
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProveedoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Proveedores
        [HttpGet]
        public async Task<IActionResult> GetProveedores()
        {
            var proveedoresEntidad = await _context.Proveedores
                .Include(p => p.Persona)
                .ToListAsync();

            var proveedoresDto = _mapper.Map<List<ProveedorDto>>(proveedoresEntidad);

            return Ok(proveedoresDto);
        }

        // GET: api/Proveedores/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProveedor(int id)
        {
            var proveedorEntidad = await _context.Proveedores
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(p => p.IdProveedor == id);

            if (proveedorEntidad == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            var proveedorDto = _mapper.Map<ProveedorDto>(proveedorEntidad);

            return Ok(proveedorDto);
        }

        // POST: api/Proveedores
        [HttpPost]
        public async Task<IActionResult> CrearProveedor([FromBody] ProveedorDto dto)
        {
            // Opcional: Validar que la Persona vinculada exista si se proporcionó IdPersona
            if (dto.IdPersona.HasValue)
            {
                var personaExiste = await _context.Personas.AnyAsync(p => p.IdPersona == dto.IdPersona.Value);
                if (!personaExiste)
                    return BadRequest(new { mensaje = "La persona especificada no existe." });
            }

            var nuevoProveedor = _mapper.Map<Proveedor>(dto);

            _context.Proveedores.Add(nuevoProveedor);
            await _context.SaveChangesAsync();

            var resultadoDto = _mapper.Map<ProveedorDto>(nuevoProveedor);

            return CreatedAtAction(nameof(GetProveedor), new { id = nuevoProveedor.IdProveedor }, resultadoDto);
        }

        // PUT: api/Proveedores/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarProveedor(int id, [FromBody] ProveedorDto dto)
        {
            var proveedorExistente = await _context.Proveedores.FindAsync(id);

            if (proveedorExistente == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            // Mapea los cambios del DTO a la entidad rastreada
            _mapper.Map(dto, proveedorExistente);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Proveedores/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Proveedor eliminado exitosamente." });
        }
    }
}