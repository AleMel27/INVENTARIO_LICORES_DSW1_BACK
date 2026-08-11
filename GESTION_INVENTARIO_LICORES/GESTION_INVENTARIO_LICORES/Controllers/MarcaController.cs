using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcaController : ControllerBase
    {
        private readonly IMarcaService _service;

        public MarcaController(IMarcaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.List();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Marca>>
                {
                    Success = true,
                    Message = "Marcas obtenidas con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron marcas registradas."
            });
        }

        [HttpGet("{idMarca}")]
        public async Task<IActionResult> GetById(long idMarca)
        {
            var marca = _service.GetMarca(idMarca);
            if (marca == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "La marca solicitada no existe."
                });
            }
            return Ok(new Response<Marca>
            {
                Success = true,
                Message = "Marca encontrada.",
                Data = marca
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Marca marca)
        {
            try
            {
                var resp = _service.Insert(marca);
                if (resp)
                {
                    return Created("", new Response<Marca>
                    {
                        Success = true,
                        Message = "Marca registrada correctamente.",
                        Data = marca
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al registrar la marca."
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
        public async Task<IActionResult> Put([FromBody] Marca marca)
        {
            var existe = _service.GetMarca(marca.IdMarca);
            if (existe == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró la marca para actualizar."
                });
            }
            try
            {
                var resp = _service.Update(marca);
                if (resp)
                {
                    return Ok(new Response<Marca>
                    {
                        Success = true,
                        Message = "Se actualizó la marca correctamente.",
                        Data = marca
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al actualizar la marca."
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

        [HttpDelete("{idMarca}")]
        public async Task<IActionResult> Delete(long idMarca)
        {
            var marca = _service.GetMarca(idMarca);
            if (marca == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró la marca para dar de baja."
                });
            }
            try
            {
                var resp = _service.Delete(idMarca);
                if (resp)
                {
                    marca.Estado = false;
                    return Ok(new Response<Marca>
                    {
                        Success = true,
                        Message = "Se dio de baja la marca correctamente.",
                        Data = marca
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al dar de baja la marca."
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