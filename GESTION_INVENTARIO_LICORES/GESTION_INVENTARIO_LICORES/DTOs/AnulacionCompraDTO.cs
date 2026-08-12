namespace GESTION_INVENTARIO_LICORES.DTOs
{
    public class AnulacionCompraDTO
    {
        public long IdUsuario { get; set; }
        public string MotivoAnulacion { get; set; } = string.Empty;
    }
}