using System;
using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Usuarios
{
    // 1. DTO para respuestas / lecturas (GET)
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public int IdPersona { get; set; }
        public string NombrePersona { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    // 2. DTO para creación (POST) - Solicita datos de la Persona + Datos de la Cuenta
    public class CrearUsuarioDto
    {
        // --- Datos de la Persona ---
        [Required(ErrorMessage = "El nombre de la persona es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        [StringLength(100)]
        public string ApPat { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ApMat { get; set; }

        [StringLength(20)]
        public string? Ci { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        // --- Datos de la Cuenta de Usuario ---
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del rol es obligatorio.")]
        public int IdRol { get; set; }
    }

    // 3. DTO para actualización (PUT)
    public class ActualizarUsuarioDto
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del rol es obligatorio.")]
        public int IdRol { get; set; }

        public bool Estado { get; set; }

        public string? NuevaContrasena { get; set; }
    }
}