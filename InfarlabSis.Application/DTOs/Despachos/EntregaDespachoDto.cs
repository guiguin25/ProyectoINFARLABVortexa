namespace InfarlabSis.Application.DTOs.Despachos
{
    public class EntregaDespachoDto
    {
        public int IdEntrega { get; set; }
        public int IdVenta { get; set; }
        public int IdUsuario { get; set; }
        public string NumeroNotaEntrega { get; set; } = string.Empty;
        public DateTime FechaEntrega { get; set; }
        public string? Observaciones { get; set; }
        public List<DetalleEntregaDto> Items { get; set; } = new();
    }

    public class DetalleEntregaDto
    {
        public int IdDetalleEntrega { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int IdLote { get; set; }
        public string? NumeroLote { get; set; }
        public decimal CantidadEntregada { get; set; }
    }
}
