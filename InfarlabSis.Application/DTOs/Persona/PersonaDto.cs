using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfarlabSis.Application.DTOs.Persona
{
    public class PersonaDto
    {
        // En las peticiones POST (Crear), el IdPersona se ignora o se envía en 0 (la BD lo genera).
        // En las peticiones GET, PUT y DELETE, se utiliza para identificar al registro.
        public int IdPersona { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [StringLength(50)]
        public string ApPat { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido materno es obligatorio.")]
        [StringLength(50)]
        public string ApMat { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CI es obligatorio.")]
        [StringLength(20)]
        public string CI { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Direccion { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;
    }
}
