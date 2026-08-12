namespace GESTION_INVENTARIO_LICORES.DTOs
{
    public class DetalleCompraRegistroDTO
    {
        public long IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
    }
}