using System.Collections.Generic;

namespace GESTION_INVENTARIO_LICORES.DTOs
{
    public class CompraRegistroDTO
    {
        public long IdProveedor { get; set; }
        public long IdUsuario { get; set; }
        public string TipoComprobante { get; set; } = string.Empty;
        public string NumeroComprobante { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public List<DetalleCompraRegistroDTO> Detalles { get; set; } = new();
    }
}