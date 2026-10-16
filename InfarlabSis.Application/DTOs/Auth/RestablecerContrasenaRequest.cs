using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Auth
{
    public class RestablecerContrasenaRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NuevaContrasena { get; set; } = string.Empty;
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
