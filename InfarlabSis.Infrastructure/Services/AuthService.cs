using InfarlabSis.Application.DTOs.Auth;
using InfarlabSis.Application.Interfaces;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

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

            if (usuario == null || !usuario.Estado) return null;

            // 2. Verificar contraseña cifrada con BCrypt
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena);
            if (!isValidPassword) return null;

            // 3. Generar JWT Token
            var token = GenerarJwtToken(usuario);

            // 4. Generar RefreshToken
            var refreshToken = new RefreshToken
            {
                IdUsuario = usuario.IdUsuario,
                Token = Guid.NewGuid().ToString("N"),
                FechaExpiracion = DateTime.UtcNow.AddDays(7),
                EsRevocado = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                Rol = usuario.Rol.Nombre
            };
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
        };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
