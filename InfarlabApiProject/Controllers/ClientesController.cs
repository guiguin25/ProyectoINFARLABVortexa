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
    [Route("api/v1/[controller]")]
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
                .Select(c => new
                {
                    idCliente = c.IdCliente,
                    razonSocial = c.RazonSocial,
                    nit = c.Nit ?? string.Empty,
                    tipoCliente = c.TipoCliente ?? string.Empty,
                    telefono = c.Telefono ?? "Sin Teléfono",
                    direccion = c.Direccion ?? "Sin Dirección"
                })
                .ToListAsync();

            return Ok(clientes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == id);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            return Ok(new
            {
                idCliente = cliente.IdCliente,
                razonSocial = cliente.RazonSocial,
                nit = cliente.Nit ?? string.Empty,
                tipoCliente = cliente.TipoCliente ?? string.Empty,
                telefono = cliente.Telefono ?? "Sin Teléfono",
                direccion = cliente.Direccion ?? "Sin Dirección"
            });
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] CrearClienteDto dto)
        {
            try
            {
                var cliente = new Cliente
                {
                    RazonSocial = dto.RazonSocial,
                    Nit = dto.Nit,
                    TipoCliente = dto.TipoCliente,
                    Direccion = dto.Direccion,
                    Telefono = dto.Telefono
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                var resultado = new
                {
                    idCliente = cliente.IdCliente,
                    razonSocial = cliente.RazonSocial,
                    nit = cliente.Nit ?? string.Empty,
                    tipoCliente = cliente.TipoCliente ?? string.Empty,
                    telefono = cliente.Telefono ?? "Sin Teléfono",
                    direccion = cliente.Direccion ?? "Sin Dirección"
                };

                return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar el cliente.", detalle = ex.Message });
            }
        }

        // PUT: api/Clientes/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ActualizarClienteDto dto)
        {
            var clienteExistente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == id);
            if (clienteExistente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            clienteExistente.RazonSocial = dto.RazonSocial;
            clienteExistente.Nit = dto.Nit;
            clienteExistente.TipoCliente = dto.TipoCliente;
            clienteExistente.Direccion = dto.Direccion;
            clienteExistente.Telefono = dto.Telefono;

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