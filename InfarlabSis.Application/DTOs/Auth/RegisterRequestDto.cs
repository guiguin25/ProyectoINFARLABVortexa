using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string ApPat { get; set; } = string.Empty;
        public string ApMat { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}