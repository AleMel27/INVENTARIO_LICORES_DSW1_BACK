using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IProductoService
{
    Task<PaginatedRespDto<ProductoRespDto>> ListAsync(
        int pageNumber = 1,
        long? idMarca = null,
        long? idCategoria = null,
        string? codigo = null,
        string? nombre = null,
        bool? estado = true,
        string orden = "DESC"
    );

    Task<ProductoRespDto?> GetByIdAsync(
        long idProducto
    );

    Task<ProductoRespDto?> CreateAsync(
        ProductoReqDto request
    );

    Task<ProductoRespDto?> UpdateAsync(
        long idProducto,
        ProductoUpdateReqDto request
    );

    Task<bool> ChangeStatusAsync(
        long idProducto,
        bool estado
    );
}
