using InfarlabSis.Application.DTOs.Auth;
using InfarlabSis.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfarlabApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);

            if (response == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });

            return Ok(response);
        }
    }
}