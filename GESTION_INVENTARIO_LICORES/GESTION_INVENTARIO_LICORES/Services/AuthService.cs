using GESTION_INVENTARIO_LICORES.DTOs.Request;
using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class AuthService : IAuthService
    {
        private readonly string conexion;
        private readonly IJwtService jwtService;

        public AuthService(
            IConfiguration configuration,
            IJwtService jwtService
        )
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );

            this.jwtService = jwtService;
        }

        public async Task<LoginRespDto?> LoginAsync(
            LoginReqDto request
        )
        {
            string correo =
                request.Correo.Trim();

            UsuarioAuthData? usuario =
                await GetByCorreoAsync(correo);

            if (usuario is null)
            {
                return null;
            }

            bool passwordCorrecta =
                BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    usuario.PasswordHash
                );

            if (!passwordCorrecta)
            {
                return null;
            }

            if (!usuario.Estado)
            {
                return null;
            }

            string token =
                jwtService.GenerateToken(
                    usuario.IdUsuario,
                    usuario.Correo,
                    usuario.NombreRol
                );

            JwtSecurityToken jwt =
                new JwtSecurityTokenHandler()
                    .ReadJwtToken(token);

            UsuarioRespDto usuarioResp =
                new UsuarioRespDto
                {
                    IdUsuario = usuario.IdUsuario,
                    Nombres = usuario.Nombres,
                    Apellidos = usuario.Apellidos,
                    Correo = usuario.Correo,

                    Rol = new RolRespDto
                    {
                        IdRol = usuario.IdRol,
                        Nombre = usuario.NombreRol
                    },

                    Estado = usuario.Estado
                };

            return new LoginRespDto
            {
                Token = token,
                TokenType = "Bearer",
                Expiracion = jwt.ValidTo,
                Usuario = usuarioResp
            };
        }

        private async Task<UsuarioAuthData?> GetByCorreoAsync(
            string correo
        )
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Usuario_ObtenerPorCorreo", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@Correo",
                        correo
                    );

                    await con.OpenAsync();

                    using (SqlDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new UsuarioAuthData
                            {
                                IdUsuario = reader.GetInt64(0),
                                Nombres = reader.GetString(1),
                                Apellidos = reader.GetString(2),
                                Correo = reader.GetString(3),
                                PasswordHash = reader.GetString(4),
                                IdRol = reader.GetInt64(5),
                                NombreRol = reader.GetString(6),
                                Estado = reader.GetBoolean(7)
                            };
                        }
                    }
                }
            }

            return null;
        }

        private sealed class UsuarioAuthData
        {
            public long IdUsuario { get; set; }

            public string Nombres { get; set; } = string.Empty;

            public string Apellidos { get; set; } = string.Empty;

            public string Correo { get; set; } = string.Empty;

            public string PasswordHash { get; set; } = string.Empty;

            public long IdRol { get; set; }

            public string NombreRol { get; set; } = string.Empty;

            public bool Estado { get; set; }
        }
    }
}
