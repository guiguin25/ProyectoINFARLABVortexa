using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfarlabSis.Application.DTOs.Actores
{
    public class ClienteDto
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El ID de la persona vinculada es obligatorio.")]
        public int IdPersona { get; set; }

        // Propiedades informativas de lectura (para las consultas GET)
        public string? NombreCompleto { get; set; }
        public string? CI { get; set; }
        public string? Email { get; set; }
    }
}
