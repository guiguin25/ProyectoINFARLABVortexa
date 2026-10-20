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
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var productosEntidad = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.Estado)
                .ToListAsync();

            var productosDto = _mapper.Map<List<ProductoDto>>(productosEntidad);

            return Ok(productosDto);
        }

        // GET: api/Productos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            var productoEntidad = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (productoEntidad == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            var productoDto = _mapper.Map<ProductoDto>(productoEntidad);

            return Ok(productoDto);
        }

        // POST: api/Productos
        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoDto dto)
        {
            // Validar que la categoría exista
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.IdCategoria == dto.IdCategoria);
            if (!categoriaExiste)
                return BadRequest(new { mensaje = "La categoría especificada no existe." });

            // Validar proveedor si fue proporcionado
            if (dto.IdProveedor.HasValue)
            {
                var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.IdProveedor == dto.IdProveedor.Value);
                if (!proveedorExiste)
                    return BadRequest(new { mensaje = "El proveedor especificado no existe." });
            }

            var nuevoProducto = _mapper.Map<Producto>(dto);
            nuevoProducto.Estado = true;

            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync();

            var resultadoDto = _mapper.Map<ProductoDto>(nuevoProducto);

            return CreatedAtAction(nameof(GetProducto), new { id = nuevoProducto.IdProducto }, resultadoDto);
        }

        // PUT: api/Productos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoDto dto)
        {
            var productoExistente = await _context.Productos.FindAsync(id);

            if (productoExistente == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            // Validar nueva categoría si cambió
            if (productoExistente.IdCategoria != dto.IdCategoria)
            {
                var categoriaExiste = await _context.Categorias.AnyAsync(c => c.IdCategoria == dto.IdCategoria);
                if (!categoriaExiste)
                    return BadRequest(new { mensaje = "La categoría especificada no existe." });
            }

            // Validar nuevo proveedor si cambió
            if (dto.IdProveedor.HasValue && productoExistente.IdProveedor != dto.IdProveedor)
            {
                var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.IdProveedor == dto.IdProveedor.Value);
                if (!proveedorExiste)
                    return BadRequest(new { mensaje = "El proveedor especificado no existe." });
            }

            _mapper.Map(dto, productoExistente);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Productos/5 (Desactivación lógica)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            producto.Estado = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto desactivado exitosamente." });
        }
    }
}