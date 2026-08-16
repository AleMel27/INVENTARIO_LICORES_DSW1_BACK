using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleCompraController : ControllerBase
    {
        private readonly IDetalleCompraService _service;

        public DetalleCompraController(
            IDetalleCompraService service
        )
        {
            _service = service;
        }

        [HttpGet("compra/{idCompra}")]
        public async Task<IActionResult> GetByCompra(
            long idCompra
        )
        {
            if (idCompra <= 0)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Solicitud inválida",
                        Detail = "El IdCompra debe ser mayor a 0."
                    }
                );
            }

            try
            {
                IReadOnlyList<DetalleCompraRespDto> detalles =
                    await _service.ListByCompraAsync(idCompra);

                return Ok(detalles);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al obtener los detalles de la compra."
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error interno del servidor."
                    }
                );
            }
        }
    }
}
