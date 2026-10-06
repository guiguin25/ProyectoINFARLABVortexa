using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Domain.Entities
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public int IdPersona { get; set; }

        public virtual Persona? Persona { get; set; }
    }
}
