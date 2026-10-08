using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Ventas
{
    public class VentaCreaDto
    {
        [Required(ErrorMessage = "La fecha de venta es obligatoria.")]
        public DateTime FechaVenta { get; set; }

        [Required(ErrorMessage = "El tipo de pago es obligatorio.")]
        [StringLength(50)]
        public string TipoPago { get; set; } = string.Empty;

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int IdCliente { get; set; }  

        [Required(ErrorMessage = "Debe haber al menos un detalle de venta.")]
        public List<DetalleVentaCreaDto> Detalles { get; set; } = new List<DetalleVentaCreaDto>();
    }
}