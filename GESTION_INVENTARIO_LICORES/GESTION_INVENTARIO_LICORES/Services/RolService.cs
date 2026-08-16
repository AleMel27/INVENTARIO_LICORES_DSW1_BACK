using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class RolService : IRolService
    {
        private readonly string conexion;

        public RolService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<IReadOnlyList<RolRespDto>> ListAsync()
        {
            List<RolRespDto> roles = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_Rol_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            roles.Add(new RolRespDto
                            {
                                IdRol = reader.GetInt64(0),
                                Nombre = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return roles;
        }
    }
}
