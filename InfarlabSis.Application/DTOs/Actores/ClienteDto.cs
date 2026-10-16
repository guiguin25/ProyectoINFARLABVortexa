using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Actores
{
    // 1. DTO para lectura (GET)
    public class ClienteDto
    {
        public int IdCliente { get; set; }
        public int IdPersona { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Ci { get; set; }
        public string? Telefono { get; set; }
    }

    // 2. DTO para creación (POST) - Solo pide los datos personales
    public class CrearClienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
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
    }

    // 3. DTO para actualización (PUT)
    public class ActualizarClienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
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
    }
}