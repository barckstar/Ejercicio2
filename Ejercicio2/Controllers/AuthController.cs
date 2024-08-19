using Ejercicio2.Interfaces;
using Ejercicio2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ejercicio2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] UserLogin model)
        {
            if (!_authService.ValidateUser(model))
            {
                return Unauthorized();
            }

            var token = _authService.GenerateToken(model);
            return Ok(new { Token = token });
        }
    }
}
