using GESTION_INVENTARIO_LICORES.DTOs;

namespace GESTION_INVENTARIO_LICORES.Interfaces
{
    public interface IMovimientoInventarioService
    {
        List<KardexReporteDto> ConsultarKardex(long? idAlmacen = null, long? idProducto = null, string? tipoMovimiento = null);
    }
}