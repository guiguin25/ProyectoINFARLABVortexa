using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Persona
    {
        public int IdPersona { get; set; }
        public string ApPat { get; set; } = string.Empty;
        public string ApMat { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

        // Relación 1:1 con Usuario
        public Usuario? Usuario { get; set; }
    }
}
