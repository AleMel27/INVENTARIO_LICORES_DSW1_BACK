using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IRolService
{
    Task<IReadOnlyList<RolRespDto>> ListAsync();
}
