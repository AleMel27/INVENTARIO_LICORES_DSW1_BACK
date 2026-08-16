using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class AlmacenService : IAlmacenService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public AlmacenService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<AlmacenRespDto>> ListAsync(
            int pageNumber = 1,
            string? nombre = null,
            string? ubicacion = null,
            bool? estado = true,
            string orden = "DESC"
        )
        {
            List<AlmacenRespDto> almacenes = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Almacen_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", (object?)nombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Ubicacion", (object?)ubicacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            almacenes.Add(new AlmacenRespDto
                            {
                                IdAlmacen = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                Ubicacion = reader.GetString(2),
                                Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Estado = reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }
            int totalItems = almacenes.Count;

            List<AlmacenRespDto> items = almacenes
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<AlmacenRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<AlmacenRespDto?> GetByIdAsync(
            long idAlmacen
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Almacen_ObtenerPorId", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new AlmacenRespDto
                            {
                                IdAlmacen = reader.GetInt64(0),
                                Nombre = reader.GetString(1),
                                Ubicacion = reader.GetString(2),
                                Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Estado = reader.GetBoolean(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<AlmacenRespDto?> CreateAsync(
            AlmacenReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Almacen_Crear", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", (object?)request.Nombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Ubicacion", (object?)request.Ubicacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)request.Descripcion ?? DBNull.Value);

                    await con.OpenAsync();

                    object? resultado = await command.ExecuteScalarAsync();

                    if (resultado is null || resultado == DBNull.Value)
                    {
                        return null;
                    }

                    long idAlmacen = Convert.ToInt64(resultado);

                    return await GetByIdAsync(idAlmacen);
                }
            }
        }

        public async Task<AlmacenRespDto?> UpdateAsync(
            long idAlmacen,
            AlmacenUpdateReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Almacen_Actualizar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@IdAlmacen",
                        idAlmacen
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

            return await GetByIdAsync(idAlmacen);
        }

        public async Task<bool> ChangeStatusAsync(
            long idAlmacen,
            bool estado
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Almacen_CambiarEstado", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@IdAlmacen",
                        idAlmacen
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
