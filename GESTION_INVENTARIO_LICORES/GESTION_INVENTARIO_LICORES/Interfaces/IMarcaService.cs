using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IMarcaService
{
    Task<PaginatedRespDto<MarcaRespDto>> ListAsync(
        int pageNumber = 1,
        string? nombre = null,
        string? paisOrigen = null,
        bool? estado = true,
        string orden = "DESC"
    );

    Task<MarcaRespDto?> GetByIdAsync(
        long idMarca
    );

    Task<MarcaRespDto?> CreateAsync(
        MarcaReqDto request
    );

    Task<MarcaRespDto?> UpdateAsync(
        long idMarca,
        MarcaUpdateReqDto request
    );

    Task<bool> ChangeStatusAsync(
        long idMarca,
        bool estado
    );
}
