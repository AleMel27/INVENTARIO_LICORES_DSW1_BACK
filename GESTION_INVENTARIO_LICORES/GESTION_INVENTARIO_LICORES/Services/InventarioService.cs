using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class InventarioService : IInventarioService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public InventarioService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<InventarioRespDto>> ListAsync(
            int pageNumber = 1,
            string? nombreProducto = null,
            string? codigoProducto = null,
            long? idAlmacen = null,
            string orden = "DESC"
        )
        {
            List<InventarioRespDto> inventarios = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Inventario_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@NombreProducto", (object?)nombreProducto ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CodigoProducto", (object?)codigoProducto ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdAlmacen", (object?)idAlmacen ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            inventarios.Add(new InventarioRespDto
                            {
                                IdInventario = reader.GetInt64(0),

                                Producto = new ProductoResumenRespDto
                                {
                                    IdProducto = reader.GetInt64(1),
                                    Codigo = reader.GetString(2),
                                    Nombre = reader.GetString(3)
                                },

                                Almacen = new AlmacenInventarioRespDto
                                {
                                    IdAlmacen = reader.GetInt64(4),
                                    Nombre = reader.GetString(5),
                                    Ubicacion = reader.GetString(6)
                                },

                                StockActual = reader.GetInt32(7)
                            });
                        }
                    }
                }
            }
            int totalItems = inventarios.Count;

            List<InventarioRespDto> items = inventarios
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<InventarioRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<InventarioRespDto?> GetByIdAsync(
            long idInventario
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Inventario_ObtenerPorId", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdInventario", idInventario);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new InventarioRespDto
                            {
                                IdInventario = reader.GetInt64(0),

                                Producto = new ProductoResumenRespDto
                                {
                                    IdProducto = reader.GetInt64(1),
                                    Codigo = reader.GetString(2),
                                    Nombre = reader.GetString(3)
                                },

                                Almacen = new AlmacenInventarioRespDto
                                {
                                    IdAlmacen = reader.GetInt64(4),
                                    Nombre = reader.GetString(5),
                                    Ubicacion = reader.GetString(6)
                                },

                                StockActual = reader.GetInt32(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<InventarioRespDto?> CreateAsync(
            InventarioReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Inventario_Crear", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProducto", request.IdProducto);
                    command.Parameters.AddWithValue("@IdAlmacen", request.IdAlmacen);

                    await con.OpenAsync();

                    object? resultado = await command.ExecuteScalarAsync();

                    if (resultado is null || resultado == DBNull.Value)
                    {
                        return null;
                    }

                    long idInventario = Convert.ToInt64(resultado);

                    return await GetByIdAsync(idInventario);
                }
            }
        }

        public async Task<bool> AdjustStockAsync(
            long idInventario,
            AjusteInventarioReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Inventario_AjustarStock", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdInventario", idInventario);
                    command.Parameters.AddWithValue("@IdUsuario", request.IdUsuario);
                    command.Parameters.AddWithValue("@IdTipoMovimiento", request.IdTipoMovimiento);
                    command.Parameters.AddWithValue("@Cantidad", request.Cantidad);
                    command.Parameters.AddWithValue("@Motivo", request.Motivo);
                    command.Parameters.AddWithValue("@Referencia", (object?)request.Referencia ?? DBNull.Value);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync();
                    }
                }
            }
        }
    }
}
