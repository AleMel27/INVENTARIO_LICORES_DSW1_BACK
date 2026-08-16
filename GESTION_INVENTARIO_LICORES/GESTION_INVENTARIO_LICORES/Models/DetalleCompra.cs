namespace GESTION_INVENTARIO_LICORES.Models
{
    public class DetalleCompra
    {
        public long IdDetalleCompra { get; set; }

        public long IdCompra { get; set; }
        public long IdProducto { get; set; }

        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}
