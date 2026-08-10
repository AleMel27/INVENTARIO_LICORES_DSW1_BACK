using Microsoft.AspNetCore.Mvc;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenController : ControllerBase
    {
        private readonly IAlmacenService _almacenService;

        public AlmacenController(IAlmacenService almacenService)
        {
            _almacenService = almacenService;
        }

        // 1. GET: api/Almacen (Listar todos los almacenes)
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var lista = _almacenService.list();
                return Ok(new { success = true, message = "Almacenes obtenidos correctamente.", data = lista });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor.", error = ex.Message });
            }
        }

        // 2. GET: api/Almacen/5 (Obtener un almacén por ID)
        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            try
            {
                var almacen = _almacenService.getAlmacen(id);
                if (almacen == null)
                {
                    return NotFound(new { success = false, message = $"No se encontró el almacén con ID {id}." });
                }
                return Ok(new { success = true, message = "Almacén encontrado.", data = almacen });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor.", error = ex.Message });
            }
        }

        // 3. POST: api/Almacen (Crear un nuevo almacén)
        [HttpPost]
        public IActionResult Post([FromBody] Almacen almacen)
        {
            try
            {
                if (almacen == null) return BadRequest(new { success = false, message = "Los datos enviados son incorrectos." });

                // Al registrar, el ID lo genera la BD (se envía 0)
                almacen.IdAlmacen = 0;

                bool insertado = _almacenService.insert(almacen);
                if (insertado)
                {
                    return CreatedAtAction(nameof(GetById), new { id = almacen.IdAlmacen }, new { success = true, message = "Almacén registrado exitosamente." });
                }

                return BadRequest(new { success = false, message = "No se pudo registrar el almacén." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor.", error = ex.Message });
            }
        }

        // 4. PUT: api/Almacen/5 (Actualizar un almacén existente)
        [HttpPut("{id}")]
        public IActionResult Put(long id, [FromBody] Almacen almacen)
        {
            try
            {
                if (almacen == null || id != almacen.IdAlmacen)
                {
                    return BadRequest(new { success = false, message = "El ID de la URL no coincide con el ID del cuerpo de la solicitud." });
                }

                // Verificar si existe antes de actualizar
                var existeAlmacen = _almacenService.getAlmacen(id);
                if (existeAlmacen == null)
                {
                    return NotFound(new { success = false, message = $"No existe el almacén con ID {id} para actualizar." });
                }

                bool actualizado = _almacenService.update(almacen);
                if (actualizado)
                {
                    return Ok(new { success = true, message = "Almacén actualizado correctamente." });
                }

                return BadRequest(new { success = false, message = "No se pudieron guardar los cambios del almacén." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor.", error = ex.Message });
            }
        }

        // 5. DELETE: api/Almacen/5 (Eliminar de forma lógica o física un almacén)
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var existeAlmacen = _almacenService.getAlmacen(id);
                if (existeAlmacen == null)
                {
                    return NotFound(new { success = false, message = $"No se encontró el almacén con ID {id}." });
                }

                bool eliminado = _almacenService.delete(id);
                if (eliminado)
                {
                    return Ok(new { success = true, message = $"Almacén con ID {id} eliminado exitosamente." });
                }

                return BadRequest(new { success = false, message = "No se pudo eliminar el almacén." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor.", error = ex.Message });
            }
        }
    }
}