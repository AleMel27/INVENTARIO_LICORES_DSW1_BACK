using GESTION_INVENTARIO_LICORES.DTOs;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public interface ICompraService
    {
        long RegistrarCompra(CompraRegistroDTO dto);

        void ProcesarRecepcion(long idCompra, RecepcionCompraDTO dto);

        void AnularCompra(long idCompra, AnulacionCompraDTO dto);
    }
}