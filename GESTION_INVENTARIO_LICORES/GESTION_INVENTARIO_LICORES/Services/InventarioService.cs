using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Exceptions;
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
            long idInventario;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                if (!await ExisteProductoActivoAsync(con, request.IdProducto))
                {
                    throw new BusinessValidationException(
                        "El producto indicado no es válido o se encuentra inactivo."
                    );
                }

                if (!await ExisteAlmacenActivoAsync(con, request.IdAlmacen))
                {
                    throw new BusinessValidationException(
                        "El almacén indicado no es válido o se encuentra inactivo."
                    );
                }

                if (await ExisteProductoAlmacenAsync(
                    con,
                    request.IdProducto,
                    request.IdAlmacen
                ))
                {
                    throw new ConflictException(
                        "El producto ya tiene un inventario registrado en ese almacén."
                    );
                }

                using (SqlTransaction transaction =
                    (SqlTransaction)await con.BeginTransactionAsync())
                {
                    try
                    {
                        using (SqlCommand command =
                            new SqlCommand(
                                "sp_Inventario_Crear",
                                con,
                                transaction
                            ))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@IdProducto", request.IdProducto);
                            command.Parameters.AddWithValue("@IdAlmacen", request.IdAlmacen);

                            object? resultado = await command.ExecuteScalarAsync();

                            if (resultado is null || resultado == DBNull.Value)
                            {
                                await transaction.RollbackAsync();
                                return null;
                            }

                            idInventario = Convert.ToInt64(resultado);
                        }

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }

            return await GetByIdAsync(idInventario);
        }

        private async Task<bool> ExisteProductoActivoAsync(
            SqlConnection con,
            long idProducto
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Producto_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdProducto", idProducto);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteAlmacenActivoAsync(
            SqlConnection con,
            long idAlmacen
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Almacen_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteProductoAlmacenAsync(
            SqlConnection con,
            long idProducto,
            long idAlmacen
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Inventario_ExisteProductoAlmacen", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdProducto", idProducto);
                command.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteInventarioAsync(
            SqlConnection con,
            long idInventario
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Inventario_ExistePorId", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdInventario", idInventario);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteUsuarioActivoAsync(
            SqlConnection con,
            long idUsuario
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Usuario_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteTipoMovimientoActivoAsync(
            SqlConnection con,
            long idTipoMovimiento
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_TipoMovimiento_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdTipoMovimiento", idTipoMovimiento);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        public async Task<bool> AdjustStockAsync(
            long idInventario,
            AjusteInventarioReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                if (!await ExisteInventarioAsync(con, idInventario))
                {
                    return false;
                }

                if (!await ExisteUsuarioActivoAsync(con, request.IdUsuario))
                {
                    throw new BusinessValidationException(
                        "El usuario indicado no es válido o se encuentra inactivo."
                    );
                }

                if (!await ExisteTipoMovimientoActivoAsync(
                    con,
                    request.IdTipoMovimiento
                ))
                {
                    throw new BusinessValidationException(
                        "El tipo de movimiento indicado no es válido o se encuentra inactivo."
                    );
                }

                using (SqlTransaction transaction =
                    (SqlTransaction)await con.BeginTransactionAsync())
                {
                    try
                    {
                        bool ajustado;

                        using (SqlCommand command =
                            new SqlCommand(
                                "sp_Inventario_AjustarStock",
                                con,
                                transaction
                            ))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@IdInventario", idInventario);
                            command.Parameters.AddWithValue("@IdUsuario", request.IdUsuario);
                            command.Parameters.AddWithValue("@IdTipoMovimiento", request.IdTipoMovimiento);
                            command.Parameters.AddWithValue("@Cantidad", request.Cantidad);
                            command.Parameters.AddWithValue("@Motivo", request.Motivo);
                            command.Parameters.AddWithValue("@Referencia", (object?)request.Referencia ?? DBNull.Value);

                            using (SqlDataReader reader =
                                await command.ExecuteReaderAsync())
                            {
                                ajustado = await reader.ReadAsync();
                            }
                        }

                        if (!ajustado)
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }

                        await transaction.CommitAsync();

                        return true;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
    }
}
