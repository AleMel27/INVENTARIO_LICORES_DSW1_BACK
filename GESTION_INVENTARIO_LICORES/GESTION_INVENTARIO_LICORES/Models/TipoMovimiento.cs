namespace GESTION_INVENTARIO_LICORES.Models
{
    public class TipoMovimiento
    {
        public long IdTipoMovimiento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
