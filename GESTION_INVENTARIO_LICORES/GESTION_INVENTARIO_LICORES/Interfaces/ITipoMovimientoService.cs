using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface ITipoMovimientoService
{
    Task<IReadOnlyList<TipoMovimientoRespDto>> ListAsync();
}
