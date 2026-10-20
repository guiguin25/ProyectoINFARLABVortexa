using Microsoft.AspNetCore.Mvc;
using InfarlabSis.Application.DTOs.Auth;
using InfarlabSis.Application.Interfaces;

namespace InfarlabApiProject.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);

            if (response == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);

            if (!response.Success)
                return BadRequest(new { mensaje = response.Mensaje });

            return Ok(new { mensaje = response.Mensaje, idUsuario = response.IdUsuario });
        }

        [HttpPost("configurar-2fa")]
        public async Task<IActionResult> Configurar2FA([FromBody] Configurar2FARequest request)
        {
            var resultado = await _authService.Configurar2FAAsync(request.IdUsuario);
            if (resultado == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(resultado);
        }

        [HttpPost("confirmar-2fa")]
        public async Task<IActionResult> Confirmar2FA([FromBody] Confirmar2FARequest request)
        {
            var exito = await _authService.Confirmar2FAAsync(request.IdUsuario, request.Codigo);
            if (!exito)
                return BadRequest(new { mensaje = "Código inválido. Intenta de nuevo." });

            return Ok(new { mensaje = "2FA activado exitosamente." });
        }

        [HttpPost("validar-totp-recuperacion")]
        public async Task<IActionResult> ValidarTotpRecuperacion([FromBody] ValidarTotpRequest request)
        {
            var token = await _authService.ValidarRecuperacion2FAAsync(request.Email, request.CodigoTotp);
            if (token == null)
                return BadRequest(new { mensaje = "Código inválido o usuario no tiene 2FA configurado." });

            return Ok(new { mensaje = "Código válido.", token, email = request.Email });
        }

        
        [HttpPost("restablecer-contrasena")]
        public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerContrasenaRequest request)
        {
            if (request.NuevaContrasena != request.ConfirmarContrasena)
                return BadRequest(new { mensaje = "Las contraseñas no coinciden." });

            var exito = await _authService.RestablecerContrasenaConTokenAsync(
                request.Email, request.Token, request.NuevaContrasena);

            if (!exito)
                return BadRequest(new { mensaje = "Token inválido o expirado." });

            return Ok(new { mensaje = "Contraseña actualizada exitosamente." });
        }

        
    }
}