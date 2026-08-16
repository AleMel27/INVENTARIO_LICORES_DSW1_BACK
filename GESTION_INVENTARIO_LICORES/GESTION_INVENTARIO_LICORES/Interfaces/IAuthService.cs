using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IAuthService
{
    Task<LoginRespDto?> LoginAsync(
        LoginReqDto request
    );
}
