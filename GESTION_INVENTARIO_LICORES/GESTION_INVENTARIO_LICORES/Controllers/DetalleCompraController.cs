using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleCompraController : ControllerBase
    {
        private readonly IDetalleCompraService _service;

        public DetalleCompraController(IDetalleCompraService service)
        {
            _service = service;
        }
        // GET: api/DetalleCompra
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = _service.ListAll();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<DetalleCompra>>
                {
                    Success = true,
                    Message = "Todos los detalles de compras obtenidos con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron detalles de compras registrados."
            });
        }

        // GET: api/DetalleCompra/compra/5
        [HttpGet("compra/{idCompra}")]
        public async Task<IActionResult> GetByCompra(long idCompra)
        {
            var lista = _service.ListByCompra(idCompra);
            if (lista.Count > 0)
            {
                return Ok(new Response<List<DetalleCompra>>
                {
                    Success = true,
                    Message = $"Detalles de la compra {idCompra} obtenidos con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron productos en el detalle de esta compra."
            });
        }

        // POST: api/DetalleCompra
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DetalleCompra detalle)
        {
            try
            {
                var resp = _service.Insert(detalle);
                if (resp)
                {
                    return Created("", new Response<DetalleCompra>
                    {
                        Success = true,
                        Message = "Producto agregado al detalle de la compra correctamente.",
                        Data = detalle
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al registrar el artículo en el detalle."
                });
            }
            catch (SqlException ex)
            {
                // Captura los RAISERROR personalizados de tu base de datos (por ejemplo, si ya no está PENDIENTE)
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}