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

        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteDto dto)
        {
            var personaExiste = await _context.Personas.AnyAsync(p => p.IdPersona == dto.IdPersona);
            if (!personaExiste)
                return BadRequest(new { mensaje = "La persona especificada no existe." });

            var nuevoCliente = new Cliente { IdPersona = dto.IdPersona };
            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cliente creado exitosamente.", idCliente = nuevoCliente.IdCliente });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ClienteDto dto)
        {
            var clienteExistente = await _context.Clientes.FindAsync(id);
            if (clienteExistente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            if (clienteExistente.IdPersona != dto.IdPersona)
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