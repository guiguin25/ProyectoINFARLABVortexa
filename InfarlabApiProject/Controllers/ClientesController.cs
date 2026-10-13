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
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ClientesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientesEntidad = await _context.Clientes
                .Include(c => c.Persona)
                .ToListAsync();

            var clientesDto = _mapper.Map<List<ClienteDto>>(clientesEntidad);

            return Ok(clientesDto);
        }

        // GET: api/Clientes/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCliente(int id)
        {
            var clienteEntidad = await _context.Clientes
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (clienteEntidad == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            var clienteDto = _mapper.Map<ClienteDto>(clienteEntidad);

            return Ok(clienteDto);
        }

        // POST: api/Clientes (Crea Persona + vincula IdPersona en Cliente)
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] CrearClienteDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear y guardar Persona
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
                await _context.SaveChangesAsync(); // SQL Server genera IdPersona

                // 2. Crear Cliente enlazado únicamente a la Persona
                var cliente = new Cliente
                {
                    IdPersona = persona.IdPersona
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Cargar datos de Persona para mapear la respuesta
                await _context.Entry(cliente).Reference(c => c.Persona).LoadAsync();

                var resultadoDto = _mapper.Map<ClienteDto>(cliente);

                return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, resultadoDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al registrar el cliente.", detalle = ex.Message });
            }
        }

        // PUT: api/Clientes/5 (Actualiza datos personales)
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

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Clientes/5
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