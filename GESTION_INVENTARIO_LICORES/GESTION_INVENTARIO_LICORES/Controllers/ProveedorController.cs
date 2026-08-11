using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _service;

        public ProveedorController(IProveedorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.List();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Proveedor>>
                {
                    Success = true,
                    Message = "Proveedores obtenidos con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron proveedores registrados."
            });
        }

        [HttpGet("{idProveedor}")]
        public async Task<IActionResult> GetById(long idProveedor)
        {
            var proveedor = _service.GetProveedor(idProveedor);
            if (proveedor == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "El proveedor solicitado no existe."
                });
            }
            return Ok(new Response<Proveedor>
            {
                Success = true,
                Message = "Proveedor encontrado.",
                Data = proveedor
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Proveedor proveedor)
        {
            try
            {
                var resp = _service.Insert(proveedor);
                if (resp)
                {
                    return Created("", new Response<Proveedor>
                    {
                        Success = true,
                        Message = "Proveedor registrado correctamente.",
                        Data = proveedor
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al registrar el proveedor."
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
        public async Task<IActionResult> Put([FromBody] Proveedor proveedor)
        {
            var existe = _service.GetProveedor(proveedor.IdProveedor);
            if (existe == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el proveedor para actualizar."
                });
            }

            try
            {
                var resp = _service.Update(proveedor);
                if (resp)
                {
                    return Ok(new Response<Proveedor>
                    {
                        Success = true,
                        Message = "Se actualizó el proveedor correctamente.",
                        Data = proveedor
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al actualizar el proveedor."
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

        [HttpDelete("{idProveedor}")]
        public async Task<IActionResult> Delete(long idProveedor)
        {
            var proveedor = _service.GetProveedor(idProveedor);
            if (proveedor == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el proveedor para dar de baja."
                });
            }

            try
            {
                var resp = _service.Delete(idProveedor);
                if (resp)
                {
                    proveedor.Estado = false;
                    return Ok(new Response<Proveedor>
                    {
                        Success = true,
                        Message = "Se dio de baja al proveedor correctamente.",
                        Data = proveedor
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al dar de baja al proveedor."
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