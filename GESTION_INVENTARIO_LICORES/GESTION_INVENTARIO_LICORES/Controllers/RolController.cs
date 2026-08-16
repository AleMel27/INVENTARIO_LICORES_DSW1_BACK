using GESTION_INVENTARIO_LICORES.DTOs.Response;
using GESTION_INVENTARIO_LICORES.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GESTION_INVENTARIO_LICORES.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly IRolService _service;

        public RolController(
            IRolService service
        )
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                IReadOnlyList<RolRespDto> roles =
                    await _service.ListAsync();

                return Ok(roles);
            }
            catch (SqlException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Error interno",
                        Detail = "Ocurrió un error al obtener los roles."
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
