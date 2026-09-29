using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        public string EmailOrUsername { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}
