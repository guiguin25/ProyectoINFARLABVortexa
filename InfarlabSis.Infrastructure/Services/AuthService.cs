using InfarlabSis.Application.DTOs.Auth;
using InfarlabSis.Application.Interfaces;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InfarlabSis.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // 1. Buscar usuario por Email o NombreUsuario
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == request.EmailOrUsername || u.NombreUsuario == request.EmailOrUsername);

            if (usuario == null || !usuario.Estado)
                return null;

            // 2. Verificar contraseña (admite texto plano en desarrollo y Hash BCrypt)
            bool esValida = false;
            if (usuario.Contrasena == request.Contrasena)
            {
                esValida = true;
            }
            else if (!string.IsNullOrEmpty(usuario.Contrasena) && usuario.Contrasena.StartsWith("$2"))
            {
                esValida = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena);
            }

            if (!esValida)
                return null;

            // 3. Generar Token JWT
            var token = GenerarJwtToken(usuario);

            return new LoginResponseDto
            {
                Token = token,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol != null ? usuario.Rol.Nombre : "Sin Rol"
            };
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var secretKey = _configuration["JwtSettings:Secret"] ?? "ClaveSecretaSuperSeguraParaInfarlabSis2026SystemJwtToken!";
            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, usuario.Rol != null ? usuario.Rol.Nombre : "Sin Rol")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}