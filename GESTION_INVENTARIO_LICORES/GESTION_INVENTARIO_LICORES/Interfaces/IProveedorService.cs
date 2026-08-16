using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IProveedorService
{
    Task<PaginatedRespDto<ProveedorRespDto>> ListAsync(
        int pageNumber = 1,
        bool? estado = true,
        string orden = "DESC"
    );

    Task<ProveedorRespDto?> GetByIdAsync(
        long idProveedor
    );

    Task<ProveedorRespDto?> CreateAsync(
        ProveedorReqDto request
    );

    Task<ProveedorRespDto?> UpdateAsync(
        long idProveedor,
        ProveedorUpdateReqDto request
    );

    Task<bool> ChangeStatusAsync(
        long idProveedor,
        bool estado
    );
}
