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
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper; // 1. Campo para AutoMapper

        // 2. Inyección de dependencias (DbContext + IMapper)
        public CategoriasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Categorias (Listar todas)
        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            // Traemos las entidades desde SQL Server
            var categoriasEntidad = await _context.Categorias
                .Where(c => c.Estado)
                .ToListAsync();

            // AutoMapper convierte la lista de Categoria a List<CategoriaDto> en una sola línea
            var categoriasDto = _mapper.Map<List<CategoriaDto>>(categoriasEntidad);

            return Ok(categoriasDto);
        }

        // GET: api/Categorias/5 (Obtener por ID)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            var categoriaEntidad = await _context.Categorias.FindAsync(id);

            if (categoriaEntidad == null)
                return NotFound(new { mensaje = "Categoría no encontrada." });

            // AutoMapper convierte la entidad individual a CategoriaDto
            var categoriaDto = _mapper.Map<CategoriaDto>(categoriaEntidad);

            return Ok(categoriaDto);
        }

        // POST: api/Categorias (Crear nueva categoría)
        [HttpPost]
        public async Task<IActionResult> CrearCategoria([FromBody] CategoriaDto dto)
        {
            // AutoMapper convierte el CategoriaDto recibido a la entidad de base de datos Categoria
            var nuevaCategoria = _mapper.Map<Categoria>(dto);
            nuevaCategoria.Estado = true;

            _context.Categorias.Add(nuevaCategoria);
            await _context.SaveChangesAsync();

            // Retornamos el DTO de la categoría creada con su nuevo Id devuelto por la BD
            var resultadoDto = _mapper.Map<CategoriaDto>(nuevaCategoria);

            return CreatedAtAction(nameof(GetCategoria), new { id = nuevaCategoria.IdCategoria }, resultadoDto);
        }

        // PUT: api/Categorias/5 (Actualizar categoría)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCategoria(int id, [FromBody] CategoriaDto dto)
        {
            var categoriaExistente = await _context.Categorias.FindAsync(id);

            if (categoriaExistente == null)
                return NotFound(new { mensaje = "Categoría no encontrada." });

            // AutoMapper copia automáticamente los datos del DTO sobre la entidad rastreada de la BD
            _mapper.Map(dto, categoriaExistente);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categorias/5 (Desactivación lógica)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound(new { mensaje = "Categoría no encontrada." });

            categoria.Estado = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Categoría desactivada exitosamente." });
        }
    }
}