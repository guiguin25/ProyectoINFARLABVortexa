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
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Persona)
                .Select(c => new
                {
                    idCliente = c.IdCliente,
                    idPersona = c.IdPersona,
                    nombre = c.Persona != null
                        ? c.Persona.Nombre + " " + c.Persona.ApPat + " " + c.Persona.ApMat
                        : "Sin Nombre",
                    telefono = c.Persona != null && c.Persona.Telefono != null
                        ? c.Persona.Telefono
                        : "Sin Teléfono",
                    email = c.Persona != null && c.Persona.Email != null
                        ? c.Persona.Email
                        : "Sin Email",
                    direccion = c.Persona != null && c.Persona.Direccion != null
                        ? c.Persona.Direccion
                        : "Sin Dirección"
                })
                .ToListAsync();

            return Ok(clientes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            return Ok(new
            {
                idCliente = cliente.IdCliente,
                idPersona = cliente.IdPersona,
                nombre = cliente.Persona != null
                    ? cliente.Persona.Nombre + " " + cliente.Persona.ApPat + " " + cliente.Persona.ApMat
                    : "Sin Nombre",
                telefono = cliente.Persona?.Telefono ?? "Sin Teléfono",
                email = cliente.Persona?.Email ?? "Sin Email",
                direccion = cliente.Persona?.Direccion ?? "Sin Dirección"
            });
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] CrearClienteDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int idPersona;

                // LÓGICA 1: Si el DTO trae un IdPersona válido, lo vinculamos (Código original)
                if (dto.IdPersona > 0)
                {
                    var personaExiste = await _context.Personas.AnyAsync(p => p.IdPersona == dto.IdPersona);
                    if (!personaExiste)
                        return BadRequest(new { mensaje = "La persona especificada no existe." });

                    idPersona = dto.IdPersona;
                }
                else
                {
                    
                    var persona = new Persona
                    {
                        Nombre = dto.Nombre,
                        ApPat = dto.ApPat,
                        ApMat = dto.ApMat,
                        CI = dto.Ci,
                        Telefono = dto.Telefono,
                        Estado = true
                    };

                    _context.Personas.Add(persona);
                    await _context.SaveChangesAsync();
                    idPersona = persona.IdPersona;
                }

                
                var cliente = new Cliente
                {
                    IdPersona = idPersona
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                
                await _context.Entry(cliente).Reference(c => c.Persona).LoadAsync();

                var resultado = new
                {
                    idCliente = cliente.IdCliente,
                    idPersona = cliente.IdPersona,
                    nombre = cliente.Persona != null ? $"{cliente.Persona.Nombre} {cliente.Persona.ApPat} {cliente.Persona.ApMat}" : "Sin Nombre",
                    telefono = cliente.Persona?.Telefono ?? "Sin Teléfono",
                    email = cliente.Persona?.Email ?? "Sin Email",
                    direccion = cliente.Persona?.Direccion ?? "Sin Dirección"
                };

                return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, resultado);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al registrar el cliente.", detalle = ex.Message });
            }
        }

        // PUT: api/Clientes/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ActualizarClienteDto dto)
        {
            var clienteExistente = await _context.Clientes
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (clienteExistente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            
            if (clienteExistente.Persona != null)
            {
                clienteExistente.Persona.Nombre = dto.Nombre;
                clienteExistente.Persona.ApPat = dto.ApPat;
                clienteExistente.Persona.ApMat = dto.ApMat;
                clienteExistente.Persona.CI = dto.Ci;
                clienteExistente.Persona.Telefono = dto.Telefono;
            }


            if (dto.IdPersona > 0 && clienteExistente.IdPersona != dto.IdPersona)
            {
                var personaExiste = await _context.Personas.AnyAsync(p => p.IdPersona == dto.IdPersona);
                if (!personaExiste)
                    return BadRequest(new { mensaje = "La persona especificada no existe." });

                clienteExistente.IdPersona = dto.IdPersona;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Cliente actualizado exitosamente." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cliente eliminado exitosamente." });
        }
    }
}