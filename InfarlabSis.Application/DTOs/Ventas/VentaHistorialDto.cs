using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Ventas
{
    public class VentaHistorialDto
    {
        public int IdHistorial { get; set; }
        public int IdVenta { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string NombreUsuarioModifico { get; set; } = string.Empty;
        public string TipoCambio { get; set; } = string.Empty;
        public string DatosAntes { get; set; } = string.Empty;
        public string DatosDespues { get; set; } = string.Empty;
        public List<DetalleVentaHistorialDto> DetallesAntes { get; set; } = new();
        public List<DetalleVentaHistorialDto> DetallesDespues { get; set; } = new();
    }
}
