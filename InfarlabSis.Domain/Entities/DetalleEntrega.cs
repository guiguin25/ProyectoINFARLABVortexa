namespace InfarlabSis.Domain.Entities
{
    public class DetalleEntrega
    {
        public int IdDetalleEntrega { get; set; }
        public int IdEntrega { get; set; }
        public int IdProducto { get; set; }
        public int IdLote { get; set; }
        public decimal CantidadEntregada { get; set; }

        public virtual EntregaDespacho Entrega { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;
        public virtual Lote Lote { get; set; } = null!;
    }
}
