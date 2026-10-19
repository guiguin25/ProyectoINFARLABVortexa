using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Actores
{
    // 1. DTO para lectura (GET)
    public class ClienteDto
    {
        public int IdCliente { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string? Nit { get; set; }
        public string? TipoCliente { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }

    // 2. DTO para creación (POST) - Solo pide los datos personales
    public class CrearClienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string RazonSocial { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Nit { get; set; }

        [StringLength(50)]
        public string? TipoCliente { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }
    }

    // 3. DTO para actualización (PUT)
    public class ActualizarClienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string RazonSocial { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Nit { get; set; }

        [StringLength(50)]
        public string? TipoCliente { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }
    }
}