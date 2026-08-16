using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IInventarioService
{
    Task<PaginatedRespDto<InventarioRespDto>> ListAsync(
        int pageNumber = 1,
        string? nombreProducto = null,
        string? codigoProducto = null,
        long? idAlmacen = null,
        string orden = "DESC"
    );

    Task<InventarioRespDto?> GetByIdAsync(
        long idInventario
    );

    Task<InventarioRespDto?> CreateAsync(
        InventarioReqDto request
    );

    Task<bool> AdjustStockAsync(
        long idInventario,
        AjusteInventarioReqDto request
    );
}
