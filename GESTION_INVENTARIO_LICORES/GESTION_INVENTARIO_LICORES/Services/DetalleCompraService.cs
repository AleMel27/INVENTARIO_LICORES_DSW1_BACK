using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class DetalleCompraService : IDetalleCompraService
    {
        private readonly string conexion;

        public DetalleCompraService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<IReadOnlyList<DetalleCompraRespDto>> ListByCompraAsync(
            long idCompra
        )
        {
            List<DetalleCompraRespDto> detalles = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_DetalleCompra_ListarPorCompra", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdCompra", idCompra);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            detalles.Add(new DetalleCompraRespDto
                            {
                                IdDetalleCompra = reader.GetInt64(0),

                                Compra = new CompraResumenRespDto
                                {
                                    IdCompra = reader.GetInt64(1),
                                    NumeroComprobante = reader.GetString(2)
                                },

                                Producto = new ProductoResumenRespDto
                                {
                                    IdProducto = reader.GetInt64(3),
                                    Codigo = reader.GetString(4),
                                    Nombre = reader.GetString(5)
                                },

                                Cantidad = reader.GetInt32(6),
                                CostoUnitario = reader.GetDecimal(7),
                                Subtotal = reader.GetDecimal(8)
                            });
                        }
                    }
                }
            }

            return detalles;
        }
    }
}
