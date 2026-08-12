using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using GESTION_INVENTARIO_LICORES.DTOs;
using GESTION_INVENTARIO_LICORES.Interfaces;

namespace GESTION_INVENTARIO_LICORES.Services
{
    public class CompraService : ICompraService
    {
        private readonly string? _conexion;

        public CompraService(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("conexion");
        }

        public long RegistrarCompra(CompraRegistroDTO dto)
        {
            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new ArgumentException("La compra debe contener al menos un producto en el detalle.");

            long idCompraGenerada = 0;

            DataTable dtDetalle = new DataTable();
            dtDetalle.Columns.Add("IdProducto", typeof(long));
            dtDetalle.Columns.Add("Cantidad", typeof(int));
            dtDetalle.Columns.Add("CostoUnitario", typeof(decimal));

            foreach (var item in dto.Detalles)
            {
                dtDetalle.Rows.Add(item.IdProducto, item.Cantidad, item.CostoUnitario);
            }

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("sp_registrar_compra", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdProveedor", dto.IdProveedor);
                    command.Parameters.AddWithValue("@IdUsuario", dto.IdUsuario);
                    command.Parameters.AddWithValue("@TipoComprobante", dto.TipoComprobante);
                    command.Parameters.AddWithValue("@NumeroComprobante", dto.NumeroComprobante);
                    command.Parameters.AddWithValue("@Observacion", (object)dto.Observacion ?? DBNull.Value);

                    SqlParameter tvpParam = command.Parameters.AddWithValue("@Detalle", dtDetalle);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.udt_DetalleCompra";

                    var id = command.ExecuteScalar();
                    if (id != null)
                    {
                        idCompraGenerada = Convert.ToInt64(id);
                    }
                }
            }

            return idCompraGenerada;
        }

        public void ProcesarRecepcion(long idCompra, RecepcionCompraDTO dto)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_procesar_recepcion_compra", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdCompra", idCompra);
                    command.Parameters.AddWithValue("@IdAlmacen", dto.IdAlmacen);
                    command.Parameters.AddWithValue("@IdUsuario", dto.IdUsuario);

                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AnularCompra(long idCompra, AnulacionCompraDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MotivoAnulacion))
                throw new ArgumentException("Debe proporcionar un motivo justificado para anular la compra.");

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                using (SqlCommand command = new SqlCommand("sp_anular_compra", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdCompra", idCompra);
                    command.Parameters.AddWithValue("@IdUsuario", dto.IdUsuario);
                    command.Parameters.AddWithValue("@MotivoAnulacion", dto.MotivoAnulacion);

                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}