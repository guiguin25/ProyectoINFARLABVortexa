using InfarlabSis.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

        //AUTHENTICATOR
        Task<object> Configurar2FAAsync(int idUsuario);
        Task<bool> Confirmar2FAAsync(int idUsuario, string codigoVerificacion);
        Task<string> ValidarRecuperacion2FAAsync(string email, string codigoTotp);
        Task<bool> RestablecerContrasenaConTokenAsync(string email, string token, string nuevaContrasena);
    }
}
