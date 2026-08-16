using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IUsuarioService
{
    Task<PaginatedRespDto<UsuarioRespDto>> ListAsync(
        int pageNumber = 1,
        string? nombres = null,
        string? apellidos = null,
        long? idRol = null,
        bool? estado = true,
        string orden = "DESC"
    );

    Task<UsuarioRespDto?> GetByIdAsync(
        long idUsuario
    );

    Task<UsuarioRespDto?> CreateAsync(
        UsuarioReqDto request
    );

    Task<UsuarioRespDto?> UpdateAsync(
        long idUsuario,
        UsuarioUpdateReqDto request
    );

    Task<bool> ChangeStatusAsync(
        long idUsuario,
        bool estado
    );
}
