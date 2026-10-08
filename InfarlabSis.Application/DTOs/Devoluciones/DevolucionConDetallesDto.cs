using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfarlabSis.Application.DTOs.Devoluciones
{
    public class DevolucionConDetallesDto
    {
        public int IdDevolucion { get; set; }

        [Required(ErrorMessage = "La fecha de devolución es obligatoria.")]
        public DateTime FechaDevolucion { get; set; }

        [Required(ErrorMessage = "El motivo de devolución es obligatorio.")]
        [StringLength(250)]
        public string MotivoDevolucion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID de venta es obligatorio.")]
        public int IdVenta { get; set; }

        public int? IdCliente { get; set; }

        public bool Estado { get; set; } = true;

        // Propiedades de navegación
        public string? NumeroVenta { get; set; }
        public string? NombreCliente { get; set; }
        public List<DetalleDevolucionDto> DetallesDevolucion { get; set; } = new List<DetalleDevolucionDto>();
    }
}