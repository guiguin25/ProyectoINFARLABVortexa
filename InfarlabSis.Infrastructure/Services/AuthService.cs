using InfarlabSis.Application.DTOs.Auth;
using InfarlabSis.Application.Interfaces;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using InfarlabSis.Infrastructure.Services;
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
        private readonly TotpService _totpService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _totpService = new TotpService();
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower()))
                    return new RegisterResponseDto { Success = false, Mensaje = "El correo electrónico ya está registrado." };

                if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario.ToLower() == request.Username.ToLower()))
                    return new RegisterResponseDto { Success = false, Mensaje = "El nombre de usuario ya está en uso." };

                if (await _context.Personas.AnyAsync(p => p.CI == request.CI))
                    return new RegisterResponseDto { Success = false, Mensaje = "La cédula de identidad ya está registrada." };

                if (!await _context.Roles.AnyAsync(r => r.IdRol == request.IdRol))
                    return new RegisterResponseDto { Success = false, Mensaje = "El rol seleccionado no es válido." };

                var nuevaPersona = new Persona
                {
                    Nombre = request.Nombre.Trim(),
                    ApPat = request.ApPat.Trim(),
                    ApMat = request.ApMat.Trim(),
                    CI = request.CI,
                    Email = request.Email.ToLower(),
                    Direccion = null,
                    Telefono = null,
                    Estado = true
                };

                _context.Personas.Add(nuevaPersona);
                await _context.SaveChangesAsync();

                var nuevoUsuario = new Usuario
                {
                    IdPersona = nuevaPersona.IdPersona,
                    NombreUsuario = request.Username.ToLower(),
                    Email = request.Email.ToLower(),
                    Contrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
                    IdRol = request.IdRol,
                    Estado = true,
                    FechaCreacion = DateTime.UtcNow,
                    TwoFactorHabilitado = false
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                return new RegisterResponseDto
                {
                    Success = true,
                    Mensaje = "Usuario y persona registrados exitosamente.",
                    IdUsuario = nuevoUsuario.IdUsuario
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponseDto { Success = false, Mensaje = "Error interno del servidor: " + ex.Message };
            }
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u =>
                    u.Email == request.EmailOrUsername ||
                    u.NombreUsuario == request.EmailOrUsername);

            if (usuario == null || !usuario.Estado)
                return null;

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

            
            string nombreCompleto = usuario.Persona != null
                ? $"{usuario.Persona.Nombre} {usuario.Persona.ApPat} {usuario.Persona.ApMat}"
                : usuario.NombreUsuario;

            return new LoginResponseDto
            {
                Token = GenerarJwtToken(usuario),
                NombreUsuario = nombreCompleto,
                Rol = usuario.Rol != null ? usuario.Rol.Nombre : "Sin Rol",
                Email = usuario.Email
            };
        }

        public async Task<object> Configurar2FAAsync(int idUsuario)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
            {
                return null;
            }

            var claveSecreta = _totpService.GenerarClaveSecreta();
            var uriQR = _totpService.GenerarUriQR(claveSecreta, usuario.Email);

            usuario.ClaveSecreta2FA = claveSecreta;
            await _context.SaveChangesAsync();

            return new { claveSecreta, uriQR, email = usuario.Email };
        }

        public async Task<bool> Confirmar2FAAsync(int idUsuario, string codigoVerificacion)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null || string.IsNullOrEmpty(usuario.ClaveSecreta2FA))
            {
                return false;
            }

            if (_totpService.ValidarCodigo(usuario.ClaveSecreta2FA, codigoVerificacion))
            {
                usuario.TwoFactorHabilitado = true;
                usuario.Estado = true;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        //VALIDA LA EXTENSIÓN "@INFARLAB.COM"
        public async Task<string> ValidarRecuperacion2FAAsync(string email, string codigoTotp)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.ToLower().EndsWith("@infarlab.com"))
            {
                return null;
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (usuario == null)
            {
                return null;
            }
            if (!usuario.TwoFactorHabilitado || string.IsNullOrEmpty(usuario.ClaveSecreta2FA))
            {
                return null;
            }

            if (_totpService.ValidarCodigo(usuario.ClaveSecreta2FA, codigoTotp))
            {
                var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
                usuario.TokenRecuperacion = token;
                usuario.ExpiracionToken = DateTime.UtcNow.AddMinutes(10);
                await _context.SaveChangesAsync();

                return token;
            }

            return null;
        }

        public async Task<bool> RestablecerContrasenaConTokenAsync(string email, string token, string nuevaContrasena)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == email.ToLower() &&
                    u.TokenRecuperacion == token &&
                    u.ExpiracionToken > DateTime.UtcNow);

            if (usuario == null)
            {
                return false;
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena);
            usuario.TokenRecuperacion = null;
            usuario.ExpiracionToken = null;
            await _context.SaveChangesAsync();

            return true;
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
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}