using System.Data;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class AlmacenService : IAlmacenService
    {
        private readonly IConfiguration _configuration;

        public AlmacenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Almacen> list()
        {
            List<Almacen> lista = new List<Almacen>();
            string cnx = _configuration.GetConnectionString("conexion");

            using (SqlConnection cn = new SqlConnection(cnx))
            {
                SqlCommand cmd = new SqlCommand("sp_list_almacenes", cn) { CommandType = CommandType.StoredProcedure };
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Almacen
                        {
                            // 1. Convertimos a Int64 porque IdAlmacen es long
                            IdAlmacen = Convert.ToInt64(dr["idAlmacen"]),
                            Nombre = dr["nombre"].ToString() ?? string.Empty,
                            Ubicacion = dr["ubicacion"].ToString() ?? string.Empty,
                            // 2. Mapeamos la propiedad opcional Descripcion controlando los nulos de la BD
                            Descripcion = dr["descripcion"] == DBNull.Value ? null : dr["descripcion"].ToString(),
                            Estado = Convert.ToBoolean(dr["estado"]),
                            FechaCreacion = Convert.ToDateTime(dr["fechaCreacion"]),
                            FechaActualizacion = Convert.ToDateTime(dr["fechaActualizacion"])
                        });
                    }
                }
            }
            return lista;
        }

        // 3. Corregido: Ahora la firma acepta 'long' tal como exige la interfaz
        public Almacen getAlmacen(long idAlmacen)
        {
            Almacen? almacen = null;
            string cnx = _configuration.GetConnectionString("conexion");

            using (SqlConnection cn = new SqlConnection(cnx))
            {
                SqlCommand cmd = new SqlCommand("sp_find_almacen_by_id", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@idAlmacen", idAlmacen);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        almacen = new Almacen
                        {
                            IdAlmacen = Convert.ToInt64(dr["idAlmacen"]),
                            Nombre = dr["nombre"].ToString() ?? string.Empty,
                            Ubicacion = dr["ubicacion"].ToString() ?? string.Empty,
                            Descripcion = dr["descripcion"] == DBNull.Value ? null : dr["descripcion"].ToString(),
                            Estado = Convert.ToBoolean(dr["estado"]),
                            FechaCreacion = Convert.ToDateTime(dr["fechaCreacion"]),
                            FechaActualizacion = Convert.ToDateTime(dr["fechaActualizacion"])
                        };
                    }
                }
            }
            return almacen!;
        }

        public bool insert(Almacen almacen)
        {
            bool respuesta = false;
            string cnx = _configuration.GetConnectionString("conexion");

            using (SqlConnection cn = new SqlConnection(cnx))
            {
                SqlCommand cmd = new SqlCommand("sp_insert_almacen", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@nombre", almacen.Nombre);
                cmd.Parameters.AddWithValue("@ubicacion", almacen.Ubicacion);
                // 4. Agregamos el parámetro @descripcion controlando si viene vacío
                cmd.Parameters.AddWithValue("@descripcion", almacen.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", almacen.Estado);

                cn.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();
                if (filasAfectadas > 0) respuesta = true;
            }
            return respuesta;
        }

        public bool update(Almacen almacen)
        {
            bool respuesta = false;
            string cnx = _configuration.GetConnectionString("conexion");

            using (SqlConnection cn = new SqlConnection(cnx))
            {
                SqlCommand cmd = new SqlCommand("sp_update_almacen", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@idAlmacen", almacen.IdAlmacen);
                cmd.Parameters.AddWithValue("@nombre", almacen.Nombre);
                cmd.Parameters.AddWithValue("@ubicacion", almacen.Ubicacion);
                cmd.Parameters.AddWithValue("@descripcion", almacen.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", almacen.Estado);

                cn.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();
                if (filasAfectadas > 0) respuesta = true;
            }
            return respuesta;
        }

        // 5. Corregido: Ahora la firma acepta 'long' tal como exige la interfaz
        public bool delete(long idAlmacen)
        {
            bool respuesta = false;
            string cnx = _configuration.GetConnectionString("conexion");

            using (SqlConnection cn = new SqlConnection(cnx))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarAlmacen", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@idAlmacen", idAlmacen);

                cn.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();
                if (filasAfectadas > 0) respuesta = true;
            }
            return respuesta;
        }
    }
}