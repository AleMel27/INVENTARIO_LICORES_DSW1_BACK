using Microsoft.AspNetCore.Mvc;
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
            var lista = _service.list();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Marca>>
                {
                    Success = true,
                    Message = "Marcas obtenidas correctamente.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron marcas."
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var marca = _service.getMarca(id);
            if (marca == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "La marca no existe."
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
        public async Task<IActionResult> Post(Marca marca)
        {
            var resp = _service.insert(marca);
            if (resp)
            {
                return Created("", new Response<Marca>
                {
                    Success = true,
                    Message = "Marca registrada con éxito.",
                    Data = marca
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "Error al registrar la marca."
            });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Marca marca)
        {
            var existente = _service.getMarca(marca.IdMarca);
            if (existente == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró la marca para actualizar."
                });
            }

            var resp = _service.update(marca);
            if (resp)
            {
                return Ok(new Response<Marca>
                {
                    Success = true,
                    Message = "Marca actualizada correctamente.",
                    Data = marca
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "Error al actualizar la marca."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var existente = _service.getMarca(id);
            if (existente == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró la marca para eliminar."
                });
            }

            var resp = _service.delete(id);
            if (resp)
            {
                return Ok(new Response<Marca>
                {
                    Success = true,
                    Message = "Marca eliminada con éxito.",
                    Data = existente
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "Error al eliminar la marca."
            });
        }
    }
}