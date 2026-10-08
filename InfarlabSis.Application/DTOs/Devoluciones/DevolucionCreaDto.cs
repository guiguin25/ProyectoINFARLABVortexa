using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Devoluciones
{
    public class DevolucionCreaDto
    {
        [Required(ErrorMessage = "La fecha de devolución es obligatoria.")]
        public DateTime FechaDevolucion { get; set; }

        [Required(ErrorMessage = "El motivo de devolución es obligatorio.")]
        [StringLength(250)]
        public string MotivoDevolucion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID de venta es obligatorio.")]
        public int IdVenta { get; set; }

        public int? IdCliente { get; set; }

        [Required(ErrorMessage = "Debe haber al menos un detalle de devolución.")]
        public List<DetalleDevolucionCreaDto> Detalles { get; set; } = new List<DetalleDevolucionCreaDto>();
    }
}