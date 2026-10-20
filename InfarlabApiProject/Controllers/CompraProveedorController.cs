using AutoMapper;
using InfarlabSis.Application.DTOs.Compras;
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
    public class CompraProveedorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CompraProveedorController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompraProveedores()
        {
            var registrosEntidad = await _context.CompraProveedores
                .Where(c => c.Estado)
                .Include(c => c.Proveedor)
                .Include(c => c.Categoria)
                .ToListAsync();

            var registrosDto = _mapper.Map<List<CompraProveedorDto>>(registrosEntidad);
            return Ok(registrosDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompraProveedor(int id)
        {
            var registroEntidad = await _context.CompraProveedores
                .Include(c => c.Proveedor)
                .Include(c => c.Categoria)
                .FirstOrDefaultAsync(c => c.IdCompra == id);

            if (registroEntidad == null)
            {
                return NotFound(new { mensaje = "Compra a proveedor no encontrada." });
            }
                
            var registroDto = _mapper.Map<CompraProveedorDto>(registroEntidad);
            return Ok(registroDto);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCompraProveedor([FromBody] CompraProveedorCreaDto dto)
        {
            var nuevoRegistro = _mapper.Map<CompraProveedor>(dto);
            nuevoRegistro.Estado = true;

            _context.CompraProveedores.Add(nuevoRegistro);
            await _context.SaveChangesAsync();

            var resultadoDto = _mapper.Map<CompraProveedorDto>(nuevoRegistro);
            return CreatedAtAction(nameof(GetCompraProveedor), new { id = nuevoRegistro.IdCompra }, resultadoDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCompraProveedor(int id, [FromBody] CompraProveedorDto dto)
        {
            var registroExistente = await _context.CompraProveedores.FindAsync(id);

            if (registroExistente == null)
            {
                return NotFound(new { mensaje = "Compra a proveedor no encontrada." });
            }
                
            _mapper.Map(dto, registroExistente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarCompraProveedor(int id)
        {
            var registro = await _context.CompraProveedores.FindAsync(id);

            if (registro == null)
            {
                return NotFound(new { mensaje = "Compra a proveedor no encontrada." });
            }
                
            registro.Estado = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Compra a proveedor desactivada exitosamente." });
        }
    }
}