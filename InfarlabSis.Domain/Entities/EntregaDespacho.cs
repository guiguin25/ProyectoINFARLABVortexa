namespace InfarlabSis.Domain.Entities
{
    public class EntregaDespacho
    {
        public int IdEntrega { get; set; }
        public int IdVenta { get; set; }
        public int IdUsuario { get; set; }
        public string NumeroNotaEntrega { get; set; } = string.Empty;
        public DateTime FechaEntrega { get; set; } = DateTime.UtcNow;
        public string? Observaciones { get; set; }

        public virtual Venta Venta { get; set; } = null!;
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual ICollection<DetalleEntrega> Detalles { get; set; } = new List<DetalleEntrega>();
    }
}
