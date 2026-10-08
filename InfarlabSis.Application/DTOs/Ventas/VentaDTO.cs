using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Ventas
{
    public class VentaDto
    {
        public int IdVenta { get; set; }

        [Required(ErrorMessage = "La fecha de venta es obligatoria.")]
        public DateTime FechaVenta { get; set; }

        [Required(ErrorMessage = "El tipo de pago es obligatorio.")]
        [StringLength(50)]
        public string TipoPago { get; set; } = string.Empty;

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        public int? IdCliente { get; set; }

        [Required(ErrorMessage = "El total es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser mayor o igual a cero.")]
        public decimal Total { get; set; }

        public bool Estado { get; set; } = true;

        //Propiedades de navegación
        public string? NombreUsuario { get; set; }
        public string? NombreCliente { get; set; }
        public List<DetalleVentaDto>? Detalles { get; set; }
    }
}