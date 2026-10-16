using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Auth
{
    public class Confirmar2FARequest
    {
        public int IdUsuario { get; set; }
        public string Codigo { get; set; } = string.Empty;
    }
}
