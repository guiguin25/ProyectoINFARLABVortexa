using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
    }
}