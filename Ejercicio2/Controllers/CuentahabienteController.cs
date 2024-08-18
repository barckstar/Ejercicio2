using Ejercicio2.Models;
using Ejercicio2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ejercicio2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CuentahabienteController : ControllerBase
    {
        private readonly CuentahabienteService _cuentahabienteService;

        public CuentahabienteController(CuentahabienteService cuentahabienteService)
        {
            _cuentahabienteService = cuentahabienteService;
        }

        // Obtener todos los cuentahabientes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cuentahabientes = await _cuentahabienteService.GetAllAsync();
            return Ok(cuentahabientes);
        }

        // Obtener un cuentahabiente por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cuentahabiente = await _cuentahabienteService.GetByIdAsync(id);
            if (cuentahabiente == null)
            {
                return NotFound(new { mensaje = "Cuentahabiente no encontrado." });
            }

            return Ok(cuentahabiente);
        }

        // Crear un nuevo cuentahabiente
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Cuentahabiente cuentahabiente)
        {
            if (cuentahabiente == null)
            {
                return BadRequest(new { mensaje = "Los datos del cuentahabiente son necesarios." });
            }

            var result = await _cuentahabienteService.CreateAsync(cuentahabiente);
            if (result == null)
            {
                return StatusCode(500, new { mensaje = "Error al crear el cuentahabiente." });
            }

            return CreatedAtAction(nameof(GetById), new { id = cuentahabiente.Id }, cuentahabiente);
        }

        // Actualizar un cuentahabiente existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Cuentahabiente cuentahabiente)
        {
            if (id != cuentahabiente.Id)
            {
                return BadRequest(new { mensaje = "ID de cuentahabiente no coincide." });
            }

            var result = await _cuentahabienteService.UpdateAsync(cuentahabiente);
            if (!result)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el cuentahabiente." });
            }

            return NoContent();
        }

        // Eliminar un cuentahabiente
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cuentahabienteService.DeleteAsync(id);
            if (!result)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el cuentahabiente." });
            }

            return NoContent();
        }
    }
}
