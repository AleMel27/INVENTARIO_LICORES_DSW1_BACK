namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Compra
    {
        public long IdCompra { get; set; }
        public long IdProveedor { get; set; }
        public long IdUsuario { get; set; }
        public long IdTipoComprobante { get; set; }

        public DateTime FechaCompra { get; set; }

        public string NumeroComprobante { get; set; } = string.Empty;

        public decimal Total { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string? Observacion { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
