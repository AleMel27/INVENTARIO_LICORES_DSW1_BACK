namespace GESTION_INVENTARIO_LICORES.DTOs
{
    public class InventarioDto
    {
        public long IdProducto { get; set; }
        public long IdAlmacen { get; set; }
        public long IdUsuario { get; set; }
        public int Cantidad { get; set; }
        public string TipoAjuste { get; set; } = null!; // 'ENTRADA' o 'SALIDA'
        public string Motivo { get; set; } = null!;
    }
}