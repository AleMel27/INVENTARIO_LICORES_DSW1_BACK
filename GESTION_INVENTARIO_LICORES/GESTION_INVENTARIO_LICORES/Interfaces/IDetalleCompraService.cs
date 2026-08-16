using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface IDetalleCompraService
{
    Task<IReadOnlyList<DetalleCompraRespDto>> ListByCompraAsync(
        long idCompra
    );
}
