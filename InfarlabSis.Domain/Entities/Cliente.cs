using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Cliente
    {
        public int IdCliente { get; set; }

        // Nueva estructura solicitada para Cliente
        public string RazonSocial { get; set; } = string.Empty;
        public string? Nit { get; set; }
        public string? TipoCliente { get; set; } // Ej. Hospital, Caja de Salud, Clínica
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }
}
