using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GESTION_INVENTARIO_LICORES.Interfaces;
using GESTION_INVENTARIO_LICORES.Models;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductoController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = _service.List();
            if (lista.Count > 0)
            {
                return Ok(new Response<List<Producto>>
                {
                    Success = true,
                    Message = "Productos obtenidos con éxito.",
                    Data = lista
                });
            }
            return BadRequest(new Response<object>
            {
                Success = false,
                Message = "No se encontraron productos registrados."
            });
        }

        [HttpGet("{idProducto}")]
        public async Task<IActionResult> GetById(long idProducto)
        {
            var producto = _service.GetProducto(idProducto);
            if (producto == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "El producto solicitado no existe."
                });
            }
            return Ok(new Response<Producto>
            {
                Success = true,
                Message = "Producto encontrado.",
                Data = producto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Producto producto)
        {
            try
            {
                var resp = _service.Insert(producto);
                if (resp)
                {
                    return Created("", new Response<Producto>
                    {
                        Success = true,
                        Message = "Producto registrado correctamente.",
                        Data = producto
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al registrar el producto."
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
        public async Task<IActionResult> Put([FromBody] Producto producto)
        {
            var existe = _service.GetProducto(producto.IdProducto);
            if (existe == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el producto para actualizar."
                });
            }

            try
            {
                var resp = _service.Update(producto);
                if (resp)
                {
                    return Ok(new Response<Producto>
                    {
                        Success = true,
                        Message = "Se actualizó el producto correctamente.",
                        Data = producto
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al actualizar el producto."
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

        [HttpDelete("{idProducto}")]
        public async Task<IActionResult> Delete(long idProducto)
        {
            var producto = _service.GetProducto(idProducto);
            if (producto == null)
            {
                return NotFound(new Response<object>
                {
                    Success = false,
                    Message = "No se encontró el producto para dar de baja."
                });
            }

            try
            {
                var resp = _service.Delete(idProducto);
                if (resp)
                {
                    producto.Estado = false;
                    return Ok(new Response<Producto>
                    {
                        Success = true,
                        Message = "Se dio de baja al producto correctamente.",
                        Data = producto
                    });
                }
                return BadRequest(new Response<object>
                {
                    Success = false,
                    Message = "Hubo un error al dar de baja al producto."
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