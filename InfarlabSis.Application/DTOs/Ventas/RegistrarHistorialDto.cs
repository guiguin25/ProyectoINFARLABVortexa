using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Ventas
{
    public class RegistrarHistorialDto
    {
        public int IdUsuario { get; set; }
        public string TipoCambio { get; set; } = string.Empty;
        public object DatosAntes { get; set; } = new();
        public object DatosDespues { get; set; } = new();
        public string? Observaciones { get; set; }
    }
}
