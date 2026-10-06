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

        // POST: api/Clientes
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteDto dto)
        {
            // Validar que la persona asociada exista en la base de datos
            var personaExiste = await _context.Personas.AnyAsync(p => p.IdPersona == dto.IdPersona);
            if (!personaExiste)
                return BadRequest(new { mensaje = "La persona especificada no existe." });

            var nuevoCliente = _mapper.Map<Cliente>(dto);

            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            var resultadoDto = _mapper.Map<ClienteDto>(nuevoCliente);

            return CreatedAtAction(nameof(GetCliente), new { id = nuevoCliente.IdCliente }, resultadoDto);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ClienteDto dto)
        {
            var clienteExistente = await _context.Clientes.FindAsync(id);

            if (clienteExistente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            // Validar si la nueva Persona existe
            if (clienteExistente.IdPersona != dto.IdPersona)
            {
                var personaExiste = await _context.Personas.AnyAsync(p => p.IdPersona == dto.IdPersona);
                if (!personaExiste)
                    return BadRequest(new { mensaje = "La persona especificada no existe." });
            }

            _mapper.Map(dto, clienteExistente);

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