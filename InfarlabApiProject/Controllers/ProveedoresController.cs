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

        // POST: api/Proveedores (Crea el Proveedor y opcionalmente la Persona de contacto)
        [HttpPost]
        public async Task<IActionResult> CrearProveedor([FromBody] CrearProveedorDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int? idPersonaCreada = null;

                // 1. Si se ingresaron datos de la persona de contacto, crear el registro de Persona primero
                if (!string.IsNullOrWhiteSpace(dto.NombreContacto) || !string.IsNullOrWhiteSpace(dto.ApPatContacto))
                {
                    var personaContacto = new Persona
                    {
                        Nombre = dto.NombreContacto ?? string.Empty,
                        ApPat = dto.ApPatContacto ?? string.Empty,
                        ApMat = dto.ApMatContacto,
                        CI = dto.CiContacto,
                        Telefono = dto.TelefonoContacto,
                        Estado = true
                    };

                    _context.Personas.Add(personaContacto);
                    await _context.SaveChangesAsync(); // Genera IdPersona

                    idPersonaCreada = personaContacto.IdPersona;
                }

                // 2. Crear el Proveedor vinculado (o no) a la Persona
                var nuevoProveedor = new Proveedor
                {
                    Nombre = dto.Nombre,
                    IdPersona = idPersonaCreada
                };

                _context.Proveedores.Add(nuevoProveedor);
                await _context.SaveChangesAsync();

                // 3. Confirmar la transacción
                await transaction.CommitAsync();

                // Cargar datos de Persona para mapear el DTO de respuesta
                await _context.Entry(nuevoProveedor).Reference(p => p.Persona).LoadAsync();

                var resultadoDto = _mapper.Map<ProveedorDto>(nuevoProveedor);

                return CreatedAtAction(nameof(GetProveedor), new { id = nuevoProveedor.IdProveedor }, resultadoDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al registrar el proveedor.", detalle = ex.Message });
            }
        }

        // PUT: api/Proveedores/5 (Actualiza Proveedor y datos de la Persona de contacto)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarProveedor(int id, [FromBody] ActualizarProveedorDto dto)
        {
            var proveedorExistente = await _context.Proveedores
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(p => p.IdProveedor == id);

            if (proveedorExistente == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            // Actualizar nombre de la empresa/proveedor
            proveedorExistente.Nombre = dto.Nombre;

            // Manejar la persona de contacto
            if (!string.IsNullOrWhiteSpace(dto.NombreContacto) || !string.IsNullOrWhiteSpace(dto.ApPatContacto))
            {
                if (proveedorExistente.Persona != null)
                {
                    // Si ya tenía persona vinculada, actualizar sus datos
                    proveedorExistente.Persona.Nombre = dto.NombreContacto ?? string.Empty;
                    proveedorExistente.Persona.ApPat = dto.ApPatContacto ?? string.Empty;
                    proveedorExistente.Persona.ApMat = dto.ApMatContacto;
                    proveedorExistente.Persona.CI = dto.CiContacto;
                    proveedorExistente.Persona.Telefono = dto.TelefonoContacto;
                }
                else
                {
                    // Si no tenía persona vinculada, crear una nueva
                    var nuevaPersona = new Persona
                    {
                        Nombre = dto.NombreContacto ?? string.Empty,
                        ApPat = dto.ApPatContacto ?? string.Empty,
                        ApMat = dto.ApMatContacto,
                        CI = dto.CiContacto,
                        Telefono = dto.TelefonoContacto,
                        Estado = true
                    };

                    _context.Personas.Add(nuevaPersona);
                    await _context.SaveChangesAsync();

                    proveedorExistente.IdPersona = nuevaPersona.IdPersona;
                }
            }

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