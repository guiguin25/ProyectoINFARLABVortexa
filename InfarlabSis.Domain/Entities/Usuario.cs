using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public int IdPersona { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty; // Se guardará el Hash seguro con BCrypt
        public int IdRol { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        //AUTHENTICATOR
        public string? ClaveSecreta2FA { get; set; }
        public bool TwoFactorHabilitado { get; set; } = false;
        public string? TokenRecuperacion { get; set; }
        public DateTime? ExpiracionToken { get; set; }

        // Propiedades de navegación
        public Persona Persona { get; set; } = null!;
        public Rol Rol { get; set; } = null!;

        // Relación 1:N con RefreshTokens
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
