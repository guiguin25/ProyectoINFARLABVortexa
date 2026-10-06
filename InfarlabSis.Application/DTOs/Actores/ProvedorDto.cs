using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfarlabSis.Application.DTOs.Actores
{
    public class ProveedorDto
    {
        public int IdProveedor { get; set; }

        public int? IdPersona { get; set; }

        [Required(ErrorMessage = "El nombre o razón social del proveedor es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        // Propiedad informativa de lectura (para consultas GET si está enlazado a una Persona)
        public string? NombrePersonaContacto { get; set; }
    }
}
