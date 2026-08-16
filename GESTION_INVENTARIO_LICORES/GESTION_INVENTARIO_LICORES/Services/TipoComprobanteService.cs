using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class TipoComprobanteService : ITipoComprobanteService
    {
        private readonly string conexion;

        public TipoComprobanteService(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("conexion")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'conexion'."
                );
        }

        public async Task<IReadOnlyList<TipoComprobanteRespDto>> ListAsync()
        {
            List<TipoComprobanteRespDto> tiposComprobante = new();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_TipoComprobante_Listar", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await con.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tiposComprobante.Add(new TipoComprobanteRespDto
                            {
                                IdTipoComprobante = reader.GetInt64(0),
                                Nombre = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return tiposComprobante;
        }
    }
}
