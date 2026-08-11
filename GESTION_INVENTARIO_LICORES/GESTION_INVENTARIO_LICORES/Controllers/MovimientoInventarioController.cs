using Microsoft.AspNetCore.Mvc;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;
using GESTION_INVENTARIO_LICORES.DTOs;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly IMovimientoInventarioService _service;

        public MovimientoInventarioController(IMovimientoInventarioService service)
        {
            _service = service;
        }

        // GET: api/MovimientoInventario
        [HttpGet]
        public async Task<IActionResult> GetKardex([FromQuery] long? idAlmacen, [FromQuery] long? idProducto, [FromQuery] string? tipoMovimiento)
        {
            var lista = _service.ConsultarKardex(idAlmacen, idProducto, tipoMovimiento);

            if (lista.Count > 0)
            {
                return Ok(new Response<List<KardexReporteDto>>
                {
                    Success = true,
                    Message = "Historial de movimientos (Kardex) consultado con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron movimientos registrados con los filtros especificados."
            });
        }
    }
}