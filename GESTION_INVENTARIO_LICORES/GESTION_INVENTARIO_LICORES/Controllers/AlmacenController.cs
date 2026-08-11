using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenController : ControllerBase
    {
        private readonly IAlmacenService _service;

        public AlmacenController(IAlmacenService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.List();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Almacen>>
                {
                    Success = true,
                    Message = "Almacenes obtenidos con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron almacenes registrados."
            });
        }

        [HttpGet("{idAlmacen}")]
        public async Task<IActionResult> GetById(long idAlmacen)
        {
            var almacen = _service.GetAlmacen(idAlmacen);
            if (almacen == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "El almacén solicitado no existe."
                });
            }
            return Ok(new Response<Almacen>
            {
                Success = true,
                Message = "Almacén encontrado.",
                Data = almacen
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Almacen almacen)
        {
            try
            {
                var resp = _service.Insert(almacen);
                if (resp)
                {
                    return Created("", new Response<Almacen>
                    {
                        Success = true,
                        Message = "Almacén registrado correctamente.",
                        Data = almacen
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al registrar el almacén."
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

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Almacen almacen)
        {
            var existe = _service.GetAlmacen(almacen.IdAlmacen);
            if (existe == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el almacén para actualizar."
                });
            }

            try
            {
                var resp = _service.Update(almacen);
                if (resp)
                {
                    return Ok(new Response<Almacen>
                    {
                        Success = true,
                        Message = "Se actualizó el almacén correctamente.",
                        Data = almacen
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al actualizar el almacén."
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

        [HttpDelete("{idAlmacen}")]
        public async Task<IActionResult> Delete(long idAlmacen)
        {
            var almacen = _service.GetAlmacen(idAlmacen);
            if (almacen == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el almacén para dar de baja."
                });
            }

            try
            {
                var resp = _service.Delete(idAlmacen);
                if (resp)
                {
                    almacen.Estado = false;
                    return Ok(new Response<Almacen>
                    {
                        Success = true,
                        Message = "Se dio de baja el almacén correctamente.",
                        Data = almacen
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al dar de baja el almacén."
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