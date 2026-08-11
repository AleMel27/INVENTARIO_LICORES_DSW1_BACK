using Microsoft.Data.SqlClient;
using System.Data;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string? _conexion;

        public UsuarioService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public List<Usuario> List()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_listar_usuarios", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Usuario
                            {
                                IdUsuario = reader.GetInt64(reader.GetOrdinal("IdUsuario")),
                                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                                Apellidos = reader.GetString(reader.GetOrdinal("Apellidos")),
                                Correo = reader.GetString(reader.GetOrdinal("Correo")),
                                Rol = reader.GetString(reader.GetOrdinal("Rol")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("FechaActualizacion"))
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Usuario GetUsuario(long idUsuario)
        {
            Usuario usuario = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_buscar_usuario", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = reader.GetInt64(reader.GetOrdinal("IdUsuario")),
                                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                                Apellidos = reader.GetString(reader.GetOrdinal("Apellidos")),
                                Correo = reader.GetString(reader.GetOrdinal("Correo")),
                                Rol = reader.GetString(reader.GetOrdinal("Rol")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("FechaActualizacion"))
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public bool Insert(Usuario usuario)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_insert_usuario", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Nombres", usuario.Nombres);
                        command.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
                        command.Parameters.AddWithValue("@Correo", usuario.Correo);
                        command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
                        command.Parameters.AddWithValue("@Rol", usuario.Rol);

                        var id = command.ExecuteScalar();
                        if (id != null)
                        {
                            usuario.IdUsuario = Convert.ToInt64(id);
                            resp = true;
                        }

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return resp;
        }

        public bool Update(Usuario usuario)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_update_usuario", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                        command.Parameters.AddWithValue("@Nombres", usuario.Nombres);
                        command.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
                        command.Parameters.AddWithValue("@Correo", usuario.Correo);
                        command.Parameters.AddWithValue("@Rol", usuario.Rol);
                        command.Parameters.AddWithValue("@Estado", usuario.Estado);

                        resp = command.ExecuteNonQuery() > 0;
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return resp;
        }

        public bool Delete(long idUsuario)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_delete_usuario", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                        resp = command.ExecuteNonQuery() > 0;
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return resp;
        }

        public bool ChangePassword(long idUsuario, string nuevoPasswordHash)
        {
            bool resp = false;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_cambiar_password", con))
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        command.Parameters.AddWithValue("@NuevoPasswordHash", nuevoPasswordHash);

                        resp = command.ExecuteNonQuery() > 0;
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return resp;
        }
    }
}