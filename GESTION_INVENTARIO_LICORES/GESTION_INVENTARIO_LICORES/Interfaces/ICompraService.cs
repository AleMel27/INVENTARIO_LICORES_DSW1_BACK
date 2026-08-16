using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;

namespace GESTION_INVENTARIO_LICORES.Interfaces;

public interface ICompraService
{
    Task<PaginatedRespDto<CompraRespDto>> ListAsync(
        int pageNumber = 1,
        string? estado = null,
        long? idTipoComprobante = null,
        DateTime? fecha = null,
        string? razonSocial = null,
        string? numeroComprobante = null,
        string orden = "DESC"
    );

    Task<CompraDetalleRespDto?> GetDetailAsync(
        long idCompra
    );

    Task<CompraDetalleRespDto?> CreateAsync(
        CompraReqDto request
    );

    Task<bool> ChangeStatusAsync(
        long idCompra,
        EstadoCompraReqDto request
    );
}
