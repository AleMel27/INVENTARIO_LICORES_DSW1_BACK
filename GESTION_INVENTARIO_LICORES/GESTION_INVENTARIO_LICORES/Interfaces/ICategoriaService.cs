using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface ICategoriaService
{
    Task<PaginatedRespDto<CategoriaRespDto>> ListAsync(
        int pageNumber = 1,
        string? nombre = null,
        bool? estado = true,
        string orden = "DESC"
    );

    Task<CategoriaRespDto?> GetByIdAsync(
        long idCategoria
    );

    Task<CategoriaRespDto?> CreateAsync(
        CategoriaReqDto request
    );

    Task<CategoriaRespDto?> UpdateAsync(
        long idCategoria,
        CategoriaUpdateReqDto request
    );

    Task<bool> ChangeStatusAsync(
        long idCategoria,
        bool estado
    );
}
