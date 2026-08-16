using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Exceptions;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class UsuarioService : IUsuarioService
    {
        private const int PageSize = 10;
        private readonly string conexion;

        public UsuarioService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<PaginatedRespDto<UsuarioRespDto>> ListAsync(
            int pageNumber = 1,
            string? nombres = null,
            string? apellidos = null,
            long? idRol = null,
            bool? estado = true,
            string orden = "DESC"
        )
        {
            List<UsuarioRespDto> usuarios = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Usuario_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombres", (object?)nombres ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Apellidos", (object?)apellidos ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdRol", (object?)idRol ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Orden", orden);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            usuarios.Add(new UsuarioRespDto
                            {
                                IdUsuario = reader.GetInt64(0),
                                Nombres = reader.GetString(1),
                                Apellidos = reader.GetString(2),
                                Correo = reader.GetString(3),

                                Rol = new RolRespDto
                                {
                                    IdRol = reader.GetInt64(4),
                                    Nombre = reader.GetString(5)
                                },

                                Estado = reader.GetBoolean(6)
                            });
                        }
                    }
                }
            }
            int totalItems = usuarios.Count;

            List<UsuarioRespDto> items = usuarios
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PaginatedRespDto<UsuarioRespDto>
            {
                Items = items,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public async Task<UsuarioRespDto?> GetByIdAsync(
            long idUsuario
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Usuario_ObtenerPorId", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new UsuarioRespDto
                            {
                                IdUsuario = reader.GetInt64(0),
                                Nombres = reader.GetString(1),
                                Apellidos = reader.GetString(2),
                                Correo = reader.GetString(3),

                                Rol = new RolRespDto
                                {
                                    IdRol = reader.GetInt64(4),
                                    Nombre = reader.GetString(5)
                                },

                                Estado = reader.GetBoolean(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<UsuarioRespDto?> CreateAsync(
            UsuarioReqDto request
        )
        {
            long idUsuario;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                if (await ExisteCorreoAsync(con, request.Correo))
                {
                    throw new ConflictException(
                        "Ya existe un usuario registrado con ese correo."
                    );
                }

                if (!await ExisteRolActivoAsync(con, request.IdRol))
                {
                    throw new BusinessValidationException(
                        "El rol indicado no es válido o se encuentra inactivo."
                    );
                }

                string passwordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        request.Password
                    );

                using (SqlTransaction transaction =
                    (SqlTransaction)await con.BeginTransactionAsync())
                {
                    try
                    {
                        using (SqlCommand command =
                            new SqlCommand(
                                "sp_Usuario_Crear",
                                con,
                                transaction
                            ))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@IdRol", request.IdRol);
                            command.Parameters.AddWithValue("@Nombres", request.Nombres);
                            command.Parameters.AddWithValue("@Apellidos", request.Apellidos);
                            command.Parameters.AddWithValue("@Correo", request.Correo);
                            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                            object? resultado = await command.ExecuteScalarAsync();

                            if (resultado is null || resultado == DBNull.Value)
                            {
                                await transaction.RollbackAsync();
                                return null;
                            }

                            idUsuario = Convert.ToInt64(resultado);
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

            return await GetByIdAsync(idUsuario);
        }

        private async Task<bool> ExisteCorreoAsync(
            SqlConnection con,
            string correo
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Usuario_ExisteCorreo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Correo", correo);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        private async Task<bool> ExisteRolActivoAsync(
            SqlConnection con,
            long idRol
        )
        {
            using (SqlCommand command =
                new SqlCommand("sp_Rol_ExistePorIdActivo", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdRol", idRol);

                object? resultado = await command.ExecuteScalarAsync();

                return resultado is not null &&
                    resultado != DBNull.Value &&
                    Convert.ToBoolean(resultado);
            }
        }

        public async Task<UsuarioRespDto?> UpdateAsync(
            long idUsuario,
            UsuarioUpdateReqDto request
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Usuario_Actualizar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@IdUsuario",
                        idUsuario
                    );

                    command.Parameters.AddWithValue(
                        "@Nombres",
                        request.Nombres
                    );

                    command.Parameters.AddWithValue(
                        "@Apellidos",
                        request.Apellidos
                    );

                    command.Parameters.AddWithValue(
                        "@Correo",
                        request.Correo
                    );

                    command.Parameters.AddWithValue(
                        "@IdRol",
                        request.IdRol
                    );

                    await con.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }

            return await GetByIdAsync(idUsuario);
        }

        public async Task<bool> ChangeStatusAsync(
            long idUsuario,
            bool estado
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Usuario_CambiarEstado", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@IdUsuario",
                        idUsuario
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
