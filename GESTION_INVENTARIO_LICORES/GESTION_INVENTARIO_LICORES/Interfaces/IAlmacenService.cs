using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IAlmacenService
{
    Task<PaginatedRespDto<AlmacenRespDto>> ListAsync(
        int pageNumber = 1,
        string? nombre = null,
        string? ubicacion = null,
        bool? estado = true,
        string orden = "DESC"
    );

    Task<AlmacenRespDto?> GetByIdAsync(
        long idAlmacen
    );

    Task<AlmacenRespDto?> CreateAsync(
        AlmacenReqDto request
    );

    Task<AlmacenRespDto?> UpdateAsync(
        long idAlmacen,
        AlmacenUpdateReqDto request
    );

    Task<bool> ChangeStatusAsync(
        long idAlmacen,
        bool estado
    );
}
