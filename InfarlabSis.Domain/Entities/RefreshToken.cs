using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class RefreshToken
    {
        public int IdRefreshToken { get; set; }
        public int IdUsuario { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public bool EsRevocado { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Propiedad de navegación
        public Usuario Usuario { get; set; } = null!;
    }
}
