using InfarlabSis.Application.DTOs.Auth;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfarlabApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // 1. Buscar usuario por Email o NombreUsuario
            var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Email == request.EmailOrUsername || u.NombreUsuario == request.EmailOrUsername);
            if (usuario == null || !usuario.Estado)
                return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });

            // 2. Verificar la contraseña con BCrypt (RNF03)
            // 2. Verificar la contraseña (admite texto plano en desarrollo y Hash BCrypt en producción)
            bool esValida = false;

            if (usuario.Contrasena == request.Contrasena)
            {
                esValida = true; // Si en SQL está guardado directamente como "123456"
            }
            else if (usuario.Contrasena.StartsWith("$2"))
            {
                esValida = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena);
            }

            if (!esValida)
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });
            // 3. Generar Token JWT con Claims de Rol (RF14)
            var token = GenerarJwtToken(usuario);

            return Ok(new LoginResponseDto
            {
                Token = token,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol.Nombre
            });
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var secretKey = _configuration["JwtSettings:Secret"] ?? "ClaveSecretaSuperSeguraParaInfarlabSis2026SystemJwtToken!";
            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["JwtSettings:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
