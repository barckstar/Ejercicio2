using Ejercicio2.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ejercicio2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajeroAutomaticoController : ControllerBase
    {
        private readonly ICajeroAutomaticoService _cajeroAutomaticoService;

        public CajeroAutomaticoController(ICajeroAutomaticoService cajeroAutomaticoService)
        {
            _cajeroAutomaticoService = cajeroAutomaticoService;
        }

        [HttpPost("retirar")]
        public async Task<IActionResult> RetirarDinero(int cuentahabienteId, decimal cantidad)
        {
            var (exito, mensaje) = await _cajeroAutomaticoService.RetirarDineroAsync(cuentahabienteId, cantidad);
            if (!exito)
            {
                return BadRequest(new { Mensaje = mensaje });
            }
            return Ok(new { Mensaje = mensaje });
        }
    }
}
