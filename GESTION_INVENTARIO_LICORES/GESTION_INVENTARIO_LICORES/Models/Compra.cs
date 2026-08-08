namespace GESTION_INVENTARIO_LICORES.Models
{
    public class Compra
    {


        public long IdCompra { get; set; }
        public long IdProveedor { get; set; }
        public long IdUsuario { get; set; }
        public DateTime FechaCompra { get; set; } = DateTime.Now;
        public string TipoComprobante { get; set; } = string.Empty;
        public string NumeroComprobante { get; set; } = string.Empty;
        public decimal Total { get; set; } = 0.00m;
        public string Estado { get; set; } = "PENDIENTE";
        public string? Observacion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Propiedades opcionales de navegación
        public Proveedor? Proveedor { get; set; }
        public Usuario? Usuario { get; set; }
        public List<DetalleCompra>? DetalleCompras { get; set; }



    }
}
