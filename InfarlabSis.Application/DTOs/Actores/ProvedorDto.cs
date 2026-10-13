using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Actores
{
    // 1. DTO para lecturas (GET)
    public class ProveedorDto
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; } = string.Empty; // Nombre de la empresa / proveedor

        // Datos de la persona de contacto (si tiene una enlazada)
        public int? IdPersona { get; set; }
        public string? NombrePersonaContacto { get; set; }
        public string? CiPersonaContacto { get; set; }
        public string? TelefonoPersonaContacto { get; set; }
    }

    // 2. DTO para creación (POST)
    public class CrearProveedorDto
    {
        [Required(ErrorMessage = "El nombre o razón social del proveedor es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        // Datos opcionales para crear la Persona de contacto
        [StringLength(100)]
        public string? NombreContacto { get; set; }

        [StringLength(100)]
        public string? ApPatContacto { get; set; }

        [StringLength(100)]
        public string? ApMatContacto { get; set; }

        [StringLength(20)]
        public string? CiContacto { get; set; }

        [StringLength(20)]
        public string? TelefonoContacto { get; set; }
    }

    // 3. DTO para actualización (PUT)
    public class ActualizarProveedorDto
    {
        [Required(ErrorMessage = "El nombre o razón social del proveedor es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        // Datos opcionales para actualizar la Persona de contacto
        [StringLength(100)]
        public string? NombreContacto { get; set; }

        [StringLength(100)]
        public string? ApPatContacto { get; set; }

        [StringLength(100)]
        public string? ApMatContacto { get; set; }

        [StringLength(20)]
        public string? CiContacto { get; set; }

        [StringLength(20)]
        public string? TelefonoContacto { get; set; }
    }
}