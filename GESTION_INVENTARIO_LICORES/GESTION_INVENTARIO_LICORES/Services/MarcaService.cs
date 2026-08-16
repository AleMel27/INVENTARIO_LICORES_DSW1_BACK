using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Exceptions;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class MarcaService : IMarcaService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public MarcaService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<MarcaRespDto>> ListAsync(
            int pageNumber = 1,
            string? nombre = null,
            string? paisOrigen = null,
            bool? estado = true,
            string orden = "DESC"
        )
        {
            List<MarcaRespDto> marcas = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Marca_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", (object?)nombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PaisOrigen", (object?)paisOrigen ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            marcas.Add(new MarcaRespDto
                            {
                                IdMarca = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                PaisOrigen = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Estado = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }
            int totalItems = marcas.Count;

            List<MarcaRespDto> items = marcas
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<MarcaRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<MarcaRespDto?> GetByIdAsync(
            long idMarca
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Marca_ObtenerPorId", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdMarca", idMarca);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new MarcaRespDto
                            {
                                IdMarca = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                PaisOrigen = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Estado = reader.GetBoolean(3)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<MarcaRespDto?> CreateAsync(
            MarcaReqDto request
        )
        {
            long idMarca;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                if (await ExisteNombreAsync(con, request.Nombre))
                {
                    throw new ConflictException(
                        "Ya existe una marca con ese nombre."
                    );
                }

                using (SqlTransaction transaction =
                    (SqlTransaction)await con.BeginTransactionAsync())
                {
                    try
                    {
                        using (SqlCommand command =
                            new SqlCommand(
                                "sp_Marca_Crear",
                                con,
                                transaction
                            ))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Nombre", request.Nombre);
                            command.Parameters.AddWithValue("@PaisOrigen", (object?)request.PaisOrigen ?? DBNull.Value);

                            object? resultado = await command.ExecuteScalarAsync();

                            if (resultado is null || resultado == DBNull.Value)
                            {
                                await transaction.RollbackAsync();
                                return null;
                            }

                            idMarca = Convert.ToInt64(resultado);
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

            return await GetByIdAsync(idMarca);
        }

        private async Task<bool> ExisteNombreAsync(
            SqlConnection con,
            string nombre
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Marca_ExisteNombre", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Nombre", nombre);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        public async Task<MarcaRespDto?> UpdateAsync(
            long idMarca,
            MarcaUpdateReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Marca_Actualizar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdMarca", idMarca);
                    command.Parameters.AddWithValue("@Nombre", request.Nombre);
                    command.Parameters.AddWithValue("@PaisOrigen", (object?)request.PaisOrigen ?? DBNull.Value);

                    await con.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    return await GetByIdAsync(idMarca);
                }
            }
        }

        public async Task<bool> ChangeStatusAsync(
            long idMarca,
            bool estado
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Marca_CambiarEstado", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdMarca", idMarca);
                    command.Parameters.AddWithValue("@Estado", estado);

                    await con.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    return true;
                }
            }
        }
    }
}
