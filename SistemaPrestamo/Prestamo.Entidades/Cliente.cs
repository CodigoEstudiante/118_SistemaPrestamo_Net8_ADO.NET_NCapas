using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prestamo.Entidades
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string NroDocumento { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string FechaCreacion { get; set; } = null!;
    }
}
