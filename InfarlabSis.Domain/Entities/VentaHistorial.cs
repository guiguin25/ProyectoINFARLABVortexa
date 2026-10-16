using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfarlabSis.Domain.Entities
{
    public class VentaHistorial
    {
        [Key]
        public int IdHistorial { get; set; }

        public int IdVenta { get; set; }

        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;

        [ForeignKey("UsuarioModifico")]
        public int IdUsuarioModifico { get; set; }

        [StringLength(50)]
        public string TipoCambio { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DatosAntes { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DatosDespues { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Observaciones { get; set; }

        //PROPIEDAD DE NAVEGACIÓN
        public Usuario? UsuarioModifico { get; set; }
    }
}