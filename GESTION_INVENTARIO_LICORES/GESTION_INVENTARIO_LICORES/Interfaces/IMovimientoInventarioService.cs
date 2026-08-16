using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IMovimientoInventarioService
{
    Task<PaginatedRespDto<MovimientoInventarioRespDto>> ListAsync(
        int pageNumber = 1,
        string? codigoProducto = null,
        string? nombreProducto = null,
        long? idAlmacen = null,
        string? numeroComprobante = null,
        long? idTipoMovimiento = null,
        string orden = "DESC"
    );

    Task<MovimientoInventarioRespDto?> GetByIdAsync(
        long idMovimiento
    );
}
