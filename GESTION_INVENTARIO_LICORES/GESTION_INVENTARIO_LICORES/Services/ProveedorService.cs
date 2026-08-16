using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class ProveedorService : IProveedorService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public ProveedorService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<ProveedorRespDto>> ListAsync(
            int pageNumber = 1,
            bool? estado = true,
            string orden = "DESC"
        )
        {
            List<ProveedorRespDto> proveedores = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Proveedor_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            proveedores.Add(new ProveedorRespDto
                            {
                                IdProveedor = reader.GetInt64(0),
                                Ruc = reader.GetString(1),
                                RazonSocial = reader.GetString(2),
                                Telefono = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Correo = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Direccion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Estado = reader.GetBoolean(6)
                            });
                        }
                    }
                }
            }
            int totalItems = proveedores.Count;

            List<ProveedorRespDto> items = proveedores
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<ProveedorRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<ProveedorRespDto?> GetByIdAsync(
            long idProveedor
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Proveedor_ObtenerPorId", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ProveedorRespDto
                            {
                                IdProveedor = reader.GetInt64(0),
                                Ruc = reader.GetString(1),
                                RazonSocial = reader.GetString(2),
                                Telefono = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Correo = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Direccion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Estado = reader.GetBoolean(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<ProveedorRespDto?> CreateAsync(
            ProveedorReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Proveedor_Crear", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Ruc", request.Ruc);
                    command.Parameters.AddWithValue("@RazonSocial", request.RazonSocial);
                    command.Parameters.AddWithValue("@Telefono", (object?)request.Telefono ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Correo", (object?)request.Correo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Direccion", (object?)request.Direccion ?? DBNull.Value);

                    await con.OpenAsync();

                    object? resultado = await command.ExecuteScalarAsync();

                    if (resultado is null || resultado == DBNull.Value)
                    {
                        return null;
                    }

                    long idProveedor = Convert.ToInt64(resultado);

                    return await GetByIdAsync(idProveedor);
                }
            }
        }

        public async Task<ProveedorRespDto?> UpdateAsync(
            long idProveedor,
            ProveedorUpdateReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Proveedor_Actualizar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);
                    command.Parameters.AddWithValue("@Ruc", request.Ruc);
                    command.Parameters.AddWithValue("@RazonSocial", request.RazonSocial);
                    command.Parameters.AddWithValue("@Telefono", (object?)request.Telefono ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Correo", (object?)request.Correo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Direccion", (object?)request.Direccion ?? DBNull.Value);

                    await con.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    return await GetByIdAsync(idProveedor);
                }
            }
        }

        public async Task<bool> ChangeStatusAsync(
            long idProveedor,
            bool estado
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Proveedor_CambiarEstado", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);
                    command.Parameters.AddWithValue("@Estado", estado);

                    await con.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    return true;
                }
            }
        }
    }
}
