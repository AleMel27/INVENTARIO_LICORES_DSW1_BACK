using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface ITipoComprobanteService
{
    Task<IReadOnlyList<TipoComprobanteRespDto>> ListAsync();
}
