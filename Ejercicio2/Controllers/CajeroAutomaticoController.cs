using Microsoft.AspNetCore.Mvc;

namespace Ejercicio2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CajeroAutomaticoController : ControllerBase
    {
        private readonly CajeroAutomaticoService _cajeroAutomaticoService;

        public CajeroAutomaticoController(CajeroAutomaticoService cajeroAutomaticoService)
        {
            _cajeroAutomaticoService = cajeroAutomaticoService;
        }

        [HttpPost("retirar")]
        public async Task<IActionResult> RetirarDinero(int cuentahabienteId, decimal cantidad)
        {
            var (success, message) = await _cajeroAutomaticoService.RetirarDineroAsync(cuentahabienteId, cantidad);
            if (!success)
            {
                return BadRequest(new { mensaje = message });
            }

            return Ok(new { mensaje = "Retiro exitoso", detalle = message });
        }
    }
}
