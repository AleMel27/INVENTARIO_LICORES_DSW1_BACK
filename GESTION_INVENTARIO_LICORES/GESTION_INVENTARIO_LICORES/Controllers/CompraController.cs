using GESTION_INVENTARIO_LICORES.DTOs;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;
using GESTION_INVENTARIO_LICORES.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _service;

        public CompraController(ICompraService service)
        {
            _service = service;
        }

        // 1. POST: api/Compra
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] CompraRegistroDTO dto)
        {
            try
            {
                long idCompra = _service.RegistrarCompra(dto);
                if (idCompra > 0)
                {
                    return Created("", new Response<object>
                    {
                        Success = true,
                        Message = "Compra registrada con éxito en el sistema.",
                        Data = new { IdCompra = idCompra }
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un problema al procesar la cabecera o el detalle de la compra."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = ex.Message
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

        // 2. PUT: api/Compra/5/recepcion
        [HttpPut("{idCompra}/recepcion")]
        public async Task<IActionResult> Recepcion(long idCompra, [FromBody] RecepcionCompraDTO dto)
        {
            try
            {
                _service.ProcesarRecepcion(idCompra, dto);
                return Ok(new Response<object>
                {
                    Success = true,
                    Message = "Recepción procesada correctamente. Stock e historial Kardex actualizados."
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

        // 3. PUT: api/Compra/5/anular
        [HttpPut("{idCompra}/anular")]
        public async Task<IActionResult> Anular(long idCompra, [FromBody] AnulacionCompraDTO dto)
        {
            try
            {
                _service.AnularCompra(idCompra, dto);
                return Ok(new Response<object>
                {
                    Success = true,
                    Message = "La compra ha sido anulada con éxito y se revirtieron los movimientos asociados."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = ex.Message
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