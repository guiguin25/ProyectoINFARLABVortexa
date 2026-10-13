using AutoMapper;
using InfarlabSis.Application.DTOs.Usuarios;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfarlabApiProject.Controllers
{
    [Authorize(Roles = "Administrador")] // Restringido solo a administradores
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsuariosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Usuarios (Listar todos los usuarios)
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuariosEntidad = await _context.Usuarios
                .Include(u => u.Persona)
                .Include(u => u.Rol)
                .Where(u => u.Estado)
                .ToListAsync();

            var usuariosDto = _mapper.Map<List<UsuarioDto>>(usuariosEntidad);

            return Ok(usuariosDto);
        }

        // GET: api/Usuarios/5 (Obtener usuario por ID)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuarioEntidad = await _context.Usuarios
                .Include(u => u.Persona)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuarioEntidad == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            var usuarioDto = _mapper.Map<UsuarioDto>(usuarioEntidad);

            return Ok(usuarioDto);
        }

        // POST: api/Usuarios (Crear nuevo usuario + persona)
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDto dto)
        {
            // 1. Validar que el Rol exista
            var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == dto.IdRol);
            if (!rolExiste)
                return BadRequest(new { mensaje = "El rol especificado no existe." });

            // 2. Validar duplicados de Email o NombreUsuario
            var existeDuplicado = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email || u.NombreUsuario == dto.NombreUsuario);
            if (existeDuplicado)
                return BadRequest(new { mensaje = "El nombre de usuario o correo ya se encuentra registrado." });

            // 3. Transacción para asegurar la consistencia entre Persona y Usuario
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. Crear y guardar los datos personales
                var nuevaPersona = new Persona
                {
                    Nombre = dto.Nombre,
                    ApPat = dto.ApPat,
                    ApMat = dto.ApMat,
                    CI = dto.Ci,
                    Telefono = dto.Telefono,
                    Estado = true
                };

                _context.Personas.Add(nuevaPersona);
                await _context.SaveChangesAsync(); // SQL Server asigna el IdPersona autoincrementable

                // B. Crear y guardar el Usuario vinculado a la Persona
                var nuevoUsuario = new Usuario
                {
                    IdPersona = nuevaPersona.IdPersona,
                    NombreUsuario = dto.NombreUsuario,
                    Email = dto.Email,
                    Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                    IdRol = dto.IdRol,
                    Estado = true,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // C. Confirmar la transacción
                await transaction.CommitAsync();

                // Cargar datos de navegación para que AutoMapper arme el UsuarioDto completo
                await _context.Entry(nuevoUsuario).Reference(u => u.Persona).LoadAsync();
                await _context.Entry(nuevoUsuario).Reference(u => u.Rol).LoadAsync();

                var resultadoDto = _mapper.Map<UsuarioDto>(nuevoUsuario);

                return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuario.IdUsuario }, resultadoDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al registrar el usuario.", detalle = ex.Message });
            }
        }

        // PUT: api/Usuarios/5 (Actualizar datos de usuario / Rol / Contraseña)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            // Validar que el nuevo Rol exista
            var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == dto.IdRol);
            if (!rolExiste)
                return BadRequest(new { mensaje = "El rol especificado no existe." });

            // Mapear campos básicos
            _mapper.Map(dto, usuarioExistente);

            // Si se envió una nueva contraseña, actualizar el Hash
            if (!string.IsNullOrWhiteSpace(dto.NuevaContrasena))
            {
                usuarioExistente.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.NuevaContrasena);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Usuarios/5 (Desactivación lógica)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            usuario.Estado = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario desactivado exitosamente." });
        }
    }
}