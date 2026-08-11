namespace GESTION_INVENTARIO_LICORES.DTOs
{
    public class KardexReporteDto
    {
        public long IdMovimiento { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Producto { get; set; } = string.Empty;
        public string Almacen { get; set; } = string.Empty;
        public string TipoMovimiento { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public int StockAnterior { get; set; }
        public int StockPosterior { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public string UsuarioResponsable { get; set; } = string.Empty;
    }
}