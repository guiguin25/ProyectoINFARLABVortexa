using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Actores
{
    // 1. DTO para lecturas (GET)
    public class ProveedorDto
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; } = string.Empty; // Nombre de la empresa / proveedor
        public string? Documento { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        // Datos de contacto interno del proveedor
        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public string? ContactoEmail { get; set; }
    }

    // 2. DTO para creación (POST)
    public class CrearProveedorDto
    {
        [Required(ErrorMessage = "El nombre o razón social del proveedor es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(20)]
        public string? Documento { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        // Datos de contacto opcionales
        [StringLength(150)]
        public string? ContactoNombre { get; set; }

        [StringLength(20)]
        public string? ContactoTelefono { get; set; }

        [EmailAddress]
        public string? ContactoEmail { get; set; }
    }

    // 3. DTO para actualización (PUT)
    public class ActualizarProveedorDto
    {
        [Required(ErrorMessage = "El nombre o razón social del proveedor es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(20)]
        public string? Documento { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        // Datos de contacto opcionales
        [StringLength(150)]
        public string? ContactoNombre { get; set; }

        [StringLength(20)]
        public string? ContactoTelefono { get; set; }

        [EmailAddress]
        public string? ContactoEmail { get; set; }
    }
}