using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Ventas
{
    public class DetalleVentaHistorialDto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string Accion { get; set; } = string.Empty;
    }
}
