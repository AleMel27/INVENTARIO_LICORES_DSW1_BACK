using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;
using GESTION_INVENTARIO_LICORES.DTOs;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _service;

        public InventarioController(IInventarioService service)
        {
            _service = service;
        }

        // GET: api/Inventario
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.List();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Inventario>>
                {
                    Success = true,
                    Message = "Inventario general obtenido con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron registros de stock en el inventario."
            });
        }

        // POST: api/Inventario/ajuste
        [HttpPost("ajuste")]
        public async Task<IActionResult> PostAjuste([FromBody] InventarioDto ajuste)
        {
            try
            {
                var resp = _service.AjustarInventario(ajuste);
                if (resp)
                {
                    return Ok(new Response<object>
                    {
                        Success = true,
                        Message = $"Ajuste de stock ({ajuste.TipoAjuste}) procesado y auditado correctamente."
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error inesperado al procesar el ajuste de inventario."
                });
            }
            catch (SqlException ex)
            {
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}