using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class MovimientoInventarioService : IMovimientoInventarioService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public MovimientoInventarioService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<MovimientoInventarioRespDto>> ListAsync(
            int pageNumber = 1,
            string? codigoProducto = null,
            string? nombreProducto = null,
            long? idAlmacen = null,
            string? numeroComprobante = null,
            long? idTipoMovimiento = null,
            string orden = "DESC"
        )
        {
            List<MovimientoInventarioRespDto> movimientos = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_MovimientoInventario_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CodigoProducto", (object?)codigoProducto ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NombreProducto", (object?)nombreProducto ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdAlmacen", (object?)idAlmacen ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroComprobante", (object?)numeroComprobante ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdTipoMovimiento", (object?)idTipoMovimiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            movimientos.Add(new MovimientoInventarioRespDto
                            {
                                IdMovimiento = reader.GetInt64(0),

                                Producto = new ProductoResumenRespDto
                                {
                                    IdProducto = reader.GetInt64(1),
                                    Codigo = reader.GetString(2),
                                    Nombre = reader.GetString(3)
                                },

                                Almacen = new AlmacenMovimientoRespDto
                                {
                                    IdAlmacen = reader.GetInt64(4),
                                    Nombre = reader.GetString(5)
                                },

                                Usuario = new UsuarioResumenRespDto
                                {
                                    IdUsuario = reader.GetInt64(6),
                                    Nombres = reader.GetString(7),
                                    Apellidos = reader.GetString(8)
                                },

                                Compra = reader.IsDBNull(9)
                                    ? null
                                    : new CompraResumenRespDto
                                    {
                                        IdCompra = reader.GetInt64(9),
                                        NumeroComprobante = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                                    },

                                TipoMovimiento = new TipoMovimientoRespDto
                                {
                                    IdTipoMovimiento = reader.GetInt64(11),
                                    Nombre = reader.GetString(12)
                                },

                                Cantidad = reader.GetInt32(13),
                                StockAnterior = reader.GetInt32(14),
                                StockPosterior = reader.GetInt32(15),
                                Motivo = reader.GetString(16),
                                Referencia = reader.IsDBNull(17) ? null : reader.GetString(17),
                                FechaMovimiento = reader.GetDateTime(18)
                            });
                        }
                    }
                }
            }
            int totalItems = movimientos.Count;

            List<MovimientoInventarioRespDto> items = movimientos
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<MovimientoInventarioRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<MovimientoInventarioRespDto?> GetByIdAsync(
            long idMovimiento
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_MovimientoInventario_ObtenerPorId", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdMovimiento", idMovimiento);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new MovimientoInventarioRespDto
                            {
                                IdMovimiento = reader.GetInt64(0),

                                Producto = new ProductoResumenRespDto
                                {
                                    IdProducto = reader.GetInt64(1),
                                    Codigo = reader.GetString(2),
                                    Nombre = reader.GetString(3)
                                },

                                Almacen = new AlmacenMovimientoRespDto
                                {
                                    IdAlmacen = reader.GetInt64(4),
                                    Nombre = reader.GetString(5)
                                },

                                Usuario = new UsuarioResumenRespDto
                                {
                                    IdUsuario = reader.GetInt64(6),
                                    Nombres = reader.GetString(7),
                                    Apellidos = reader.GetString(8)
                                },

                                Compra = reader.IsDBNull(9)
                                    ? null
                                    : new CompraResumenRespDto
                                    {
                                        IdCompra = reader.GetInt64(9),
                                        NumeroComprobante = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                                    },

                                TipoMovimiento = new TipoMovimientoRespDto
                                {
                                    IdTipoMovimiento = reader.GetInt64(11),
                                    Nombre = reader.GetString(12)
                                },

                                Cantidad = reader.GetInt32(13),
                                StockAnterior = reader.GetInt32(14),
                                StockPosterior = reader.GetInt32(15),
                                Motivo = reader.GetString(16),
                                Referencia = reader.IsDBNull(17) ? null : reader.GetString(17),
                                FechaMovimiento = reader.GetDateTime(18)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
