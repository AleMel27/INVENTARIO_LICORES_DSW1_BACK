using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Exceptions;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class ProductoService : IProductoService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public ProductoService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<ProductoRespDto>> ListAsync(
            int pageNumber = 1,
            long? idMarca = null,
            long? idCategoria = null,
            string? codigo = null,
            string? nombre = null,
            bool? estado = true,
            string orden = "DESC"
        )
        {
            List<ProductoRespDto> productos = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Producto_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdMarca", (object?)idMarca ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdCategoria", (object?)idCategoria ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Codigo", (object?)codigo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Nombre", (object?)nombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productos.Add(new ProductoRespDto
                            {
                                IdProducto = reader.GetInt64(0),
                                Codigo = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CapacidadMl = reader.GetInt32(4),
                                GradoAlcoholico = reader.GetDecimal(5),
                                PrecioVenta = reader.GetDecimal(6),
                                StockMinimo = reader.GetInt32(7),

                                Categoria = new CategoriaResumenRespDto
                                {
                                    IdCategoria = reader.GetInt64(8),
                                    Nombre = reader.GetString(9)
                                },

                                Marca = new MarcaResumenRespDto
                                {
                                    IdMarca = reader.GetInt64(10),
                                    Nombre = reader.GetString(11)
                                },

                                Estado = reader.GetBoolean(12)
                            });
                        }
                    }
                }
            }
            int totalItems = productos.Count;

            List<ProductoRespDto> items = productos
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<ProductoRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<ProductoRespDto?> GetByIdAsync(
            long idProducto
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Producto_ObtenerPorId", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdProducto", idProducto);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ProductoRespDto
                            {
                                IdProducto = reader.GetInt64(0),
                                Codigo = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CapacidadMl = reader.GetInt32(4),
                                GradoAlcoholico = reader.GetDecimal(5),
                                PrecioVenta = reader.GetDecimal(6),
                                StockMinimo = reader.GetInt32(7),

                                Categoria = new CategoriaResumenRespDto
                                {
                                    IdCategoria = reader.GetInt64(8),
                                    Nombre = reader.GetString(9)
                                },

                                Marca = new MarcaResumenRespDto
                                {
                                    IdMarca = reader.GetInt64(10),
                                    Nombre = reader.GetString(11)
                                },

                                Estado = reader.GetBoolean(12)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<ProductoRespDto?> CreateAsync(
            ProductoReqDto request
        )
        {
            long idProducto;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                if (await ExisteCodigoAsync(con, request.Codigo))
                {
                    throw new ConflictException(
                        "Ya existe un producto registrado con ese código."
                    );
                }

                if (!await ExisteCategoriaActivaAsync(con, request.IdCategoria))
                {
                    throw new BusinessValidationException(
                        "La categoría indicada no es válida o se encuentra inactiva."
                    );
                }

                if (!await ExisteMarcaActivaAsync(con, request.IdMarca))
                {
                    throw new BusinessValidationException(
                        "La marca indicada no es válida o se encuentra inactiva."
                    );
                }

                using (SqlTransaction transaction =
                    (SqlTransaction)await con.BeginTransactionAsync())
                {
                    try
                    {
                        using (SqlCommand command =
                            new SqlCommand(
                                "sp_Producto_Crear",
                                con,
                                transaction
                            ))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@IdCategoria", request.IdCategoria);
                            command.Parameters.AddWithValue("@IdMarca", request.IdMarca);
                            command.Parameters.AddWithValue("@Codigo", request.Codigo);
                            command.Parameters.AddWithValue("@Nombre", request.Nombre);
                            command.Parameters.AddWithValue("@Descripcion", (object?)request.Descripcion ?? DBNull.Value);
                            command.Parameters.AddWithValue("@CapacidadMl", request.CapacidadMl);
                            command.Parameters.AddWithValue("@GradoAlcoholico", request.GradoAlcoholico);
                            command.Parameters.AddWithValue("@PrecioVenta", request.PrecioVenta);
                            command.Parameters.AddWithValue("@StockMinimo", request.StockMinimo);

                            object? resultado = await command.ExecuteScalarAsync();

                            if (resultado is null || resultado == DBNull.Value)
                            {
                                await transaction.RollbackAsync();
                                return null;
                            }

                            idProducto = Convert.ToInt64(resultado);
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

            return await GetByIdAsync(idProducto);
        }

        private async Task<bool> ExisteCodigoAsync(
            SqlConnection con,
            string codigo
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Producto_ExisteCodigo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Codigo", codigo);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteCategoriaActivaAsync(
            SqlConnection con,
            long idCategoria
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Categoria_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdCategoria", idCategoria);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteMarcaActivaAsync(
            SqlConnection con,
            long idMarca
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Marca_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdMarca", idMarca);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<ProductoRespDto?> ObtenerPorIdAsync(
            SqlConnection con,
            long idProducto
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Producto_ObtenerPorId", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdProducto", idProducto);

                using (SqlDataReader reader =
                    await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new ProductoRespDto
                        {
                            IdProducto = reader.GetInt64(0),
                            Codigo = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CapacidadMl = reader.GetInt32(4),
                            GradoAlcoholico = reader.GetDecimal(5),
                            PrecioVenta = reader.GetDecimal(6),
                            StockMinimo = reader.GetInt32(7),

                            Categoria = new CategoriaResumenRespDto
                            {
                                IdCategoria = reader.GetInt64(8),
                                Nombre = reader.GetString(9)
                            },

                            Marca = new MarcaResumenRespDto
                            {
                                IdMarca = reader.GetInt64(10),
                                Nombre = reader.GetString(11)
                            },

                            Estado = reader.GetBoolean(12)
                        };
                    }
                }
            }

            return null;
        }

        public async Task<ProductoRespDto?> UpdateAsync(
            long idProducto,
            ProductoUpdateReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                ProductoRespDto? productoActual =
                    await ObtenerPorIdAsync(
                        con,
                        idProducto
                    );

                if (productoActual is null)
                {
                    return null;
                }

                if (!await ExisteCategoriaActivaAsync(con, request.IdCategoria))
                {
                    throw new BusinessValidationException(
                        "La categoría indicada no es válida o se encuentra inactiva."
                    );
                }

                if (!await ExisteMarcaActivaAsync(con, request.IdMarca))
                {
                    throw new BusinessValidationException(
                        "La marca indicada no es válida o se encuentra inactiva."
                    );
                }

                using (SqlCommand command = new SqlCommand("sp_Producto_Actualizar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProducto", idProducto);
                    command.Parameters.AddWithValue("@IdCategoria", request.IdCategoria);
                    command.Parameters.AddWithValue("@IdMarca", request.IdMarca);
                    command.Parameters.AddWithValue("@Nombre", request.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", (object?)request.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CapacidadMl", request.CapacidadMl);
                    command.Parameters.AddWithValue("@GradoAlcoholico", request.GradoAlcoholico);
                    command.Parameters.AddWithValue("@PrecioVenta", request.PrecioVenta);
                    command.Parameters.AddWithValue("@StockMinimo", request.StockMinimo);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await GetByIdAsync(idProducto);
        }

        public async Task<bool> ChangeStatusAsync(
            long idProducto,
            bool estado
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                ProductoRespDto? productoActual =
                    await ObtenerPorIdAsync(
                        con,
                        idProducto
                    );

                if (productoActual is null)
                {
                    return false;
                }

                if (productoActual.Estado == estado)
                {
                    throw new ConflictException(
                        estado
                            ? "El producto ya se encuentra activo."
                            : "El producto ya se encuentra inactivo."
                    );
                }

                using (SqlCommand command = new SqlCommand("sp_Producto_CambiarEstado", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProducto", idProducto);
                    command.Parameters.AddWithValue("@Estado", estado);

                    await command.ExecuteNonQueryAsync();

                    return true;
                }
            }
        }
    }
}
