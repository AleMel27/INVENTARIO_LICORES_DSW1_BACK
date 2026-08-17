using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Exceptions;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class CategoriaService : ICategoriaService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public CategoriaService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<CategoriaRespDto>> ListAsync(
            int pageNumber = 1,
            string? nombre = null,
            bool? estado = true,
            string orden = "DESC"
        )
        {
            List<CategoriaRespDto> categorias = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Categoria_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", (object?)nombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categorias.Add(new CategoriaRespDto
                            {
                                IdCategoria = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Estado = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }
            int totalItems = categorias.Count;

            List<CategoriaRespDto> items = categorias
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<CategoriaRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<CategoriaRespDto?> GetByIdAsync(
            long idCategoria
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Categoria_ObtenerPorId", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdCategoria", idCategoria);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new CategoriaRespDto
                            {
                                IdCategoria = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Estado = reader.GetBoolean(3)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<CategoriaRespDto?> CreateAsync(
            CategoriaReqDto request
        )
        {
            long idCategoria;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                if (await ExisteNombreAsync(con, request.Nombre))
                {
                    throw new ConflictException(
                        "Ya existe una categoría con ese nombre."
                    );
                }

                using (SqlTransaction transaction =
                    (SqlTransaction)await con.BeginTransactionAsync())
                {
                    try
                    {
                        using (SqlCommand command =
                            new SqlCommand(
                                "sp_Categoria_Crear",
                                con,
                                transaction
                            ))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Nombre", (object?)request.Nombre ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Descripcion", (object?)request.Descripcion ?? DBNull.Value);

                            object? resultado = await command.ExecuteScalarAsync();

                            if (resultado is null || resultado == DBNull.Value)
                            {
                                await transaction.RollbackAsync();
                                return null;
                            }

                            idCategoria = Convert.ToInt64(resultado);
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

            return await GetByIdAsync(idCategoria);
        }

        private async Task<bool> ExisteNombreAsync(
            SqlConnection con,
            string nombre,
            long? idCategoria = null
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Categoria_ExisteNombre", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Nombre", nombre);

                if (idCategoria.HasValue)
                {
                    command.Parameters.AddWithValue(
                        "@IdCategoria",
                        idCategoria.Value
                    );
                }

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<CategoriaRespDto?> ObtenerPorIdAsync(
            SqlConnection con,
            long idCategoria
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Categoria_ObtenerPorId", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdCategoria", idCategoria);

                using (SqlDataReader reader =
                    await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new CategoriaRespDto
                        {
                            IdCategoria = reader.GetInt64(0),
                            Nombre = reader.GetString(1),
                            Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Estado = reader.GetBoolean(3)
                        };
                    }
                }
            }

            return null;
        }

        public async Task<CategoriaRespDto?> UpdateAsync(
            long idCategoria,
            CategoriaUpdateReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                CategoriaRespDto? categoriaActual =
                    await ObtenerPorIdAsync(
                        con,
                        idCategoria
                    );

                if (categoriaActual is null)
                {
                    return null;
                }

                if (await ExisteNombreAsync(
                    con,
                    request.Nombre,
                    idCategoria
                ))
                {
                    throw new ConflictException(
                        "Ya existe una categoría con ese nombre."
                    );
                }

                using (SqlCommand command =
                    new SqlCommand("sp_Categoria_Actualizar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@IdCategoria",
                        idCategoria
                    );

                    command.Parameters.AddWithValue(
                        "@Nombre",
                        request.Nombre
                    );

                    command.Parameters.AddWithValue(
                        "@Descripcion",
                        (object?)request.Descripcion ?? DBNull.Value
                    );

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await GetByIdAsync(idCategoria);
        }

        public async Task<bool> ChangeStatusAsync(
            long idCategoria,
            bool estado
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                CategoriaRespDto? categoriaActual =
                    await ObtenerPorIdAsync(
                        con,
                        idCategoria
                    );

                if (categoriaActual is null)
                {
                    return false;
                }

                if (categoriaActual.Estado == estado)
                {
                    throw new ConflictException(
                        estado
                            ? "La categoría ya se encuentra activa."
                            : "La categoría ya se encuentra inactiva."
                    );
                }

                using (SqlCommand command =
                    new SqlCommand("sp_Categoria_CambiarEstado", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@IdCategoria",
                        idCategoria
                    );

                    command.Parameters.AddWithValue(
                        "@Estado",
                        estado
                    );

                    await command.ExecuteNonQueryAsync();

                    return true;
                }
            }
        }
    }
}
