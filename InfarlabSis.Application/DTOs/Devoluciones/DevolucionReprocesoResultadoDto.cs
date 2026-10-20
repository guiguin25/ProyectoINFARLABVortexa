namespace InfarlabSis.Application.DTOs.Devoluciones
{
    public class DevolucionReprocesoResultadoDto
    {
        public int IdDevolucion { get; set; }
        public int IdLoteReproceso { get; set; }
        public string NumeroLoteReproceso { get; set; } = string.Empty;
        public int IdAlmacenCuarentena { get; set; }
        public decimal Cantidad { get; set; }
    }
}
