using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
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
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Categoria_Crear", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", (object?)request.Nombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)request.Descripcion ?? DBNull.Value);

                    await con.OpenAsync();

                    object? resultado = await command.ExecuteScalarAsync();

                    if (resultado is null || resultado == DBNull.Value)
                    {
                        return null;
                    }

                    long idCategoria = Convert.ToInt64(resultado);

                    return await GetByIdAsync(idCategoria);
                }
            }
        }

        public async Task<CategoriaRespDto?> UpdateAsync(
            long idCategoria,
            CategoriaUpdateReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
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

                    await con.OpenAsync();

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

                    await con.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    return true;
                }
            }
        }
    }
}
