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
            var proveedoresEntidad = await _context.Proveedores.ToListAsync();
            var proveedoresDto = _mapper.Map<List<ProveedorDto>>(proveedoresEntidad);
            return Ok(proveedoresDto);
        }

        // GET: api/Proveedores/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProveedor(int id)
        {
            var proveedorEntidad = await _context.Proveedores.FirstOrDefaultAsync(p => p.IdProveedor == id);
            if (proveedorEntidad == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });
            var proveedorDto = _mapper.Map<ProveedorDto>(proveedorEntidad);
            return Ok(proveedorDto);
        }

        // POST: api/Proveedores (Crea el Proveedor y opcionalmente la Persona de contacto)
        [HttpPost]
        public async Task<IActionResult> CrearProveedor([FromBody] CrearProveedorDto dto)
        {
            try
            {
                var nuevoProveedor = new Proveedor
                {
                    Nombre = dto.Nombre,
                    Documento = dto.Documento,
                    Direccion = dto.Direccion,
                    Telefono = dto.Telefono,
                    Email = dto.Email,
                    ContactoNombre = dto.ContactoNombre,
                    ContactoTelefono = dto.ContactoTelefono,
                    ContactoEmail = dto.ContactoEmail
                };

                _context.Proveedores.Add(nuevoProveedor);
                await _context.SaveChangesAsync();

                var resultadoDto = _mapper.Map<ProveedorDto>(nuevoProveedor);
                return CreatedAtAction(nameof(GetProveedor), new { id = nuevoProveedor.IdProveedor }, resultadoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar el proveedor.", detalle = ex.Message });
            }
        }

        // PUT: api/Proveedores/5 (Actualiza Proveedor y datos de la Persona de contacto)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarProveedor(int id, [FromBody] ActualizarProveedorDto dto)
        {
            var proveedorExistente = await _context.Proveedores.FirstOrDefaultAsync(p => p.IdProveedor == id);
            if (proveedorExistente == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            proveedorExistente.Nombre = dto.Nombre;
            proveedorExistente.Documento = dto.Documento;
            proveedorExistente.Direccion = dto.Direccion;
            proveedorExistente.Telefono = dto.Telefono;
            proveedorExistente.Email = dto.Email;
            proveedorExistente.ContactoNombre = dto.ContactoNombre;
            proveedorExistente.ContactoTelefono = dto.ContactoTelefono;
            proveedorExistente.ContactoEmail = dto.ContactoEmail;

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