using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.DTOs.Auth
{
    public class ValidarTotpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string CodigoTotp { get; set; } = string.Empty;
    }
}
