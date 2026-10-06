using InfarlabSis.Application.DTOs.Usuarios;
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

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Select(u => new
                {
                    u.IdUsuario,
                    u.NombreUsuario,
                    u.Email,
                    Rol = u.Rol.Nombre,
                    u.Estado
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioDto dto)
        {
            // Hashear contraseña con BCrypt antes de guardar
            // var hash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            return Ok();
        }
    }
}
