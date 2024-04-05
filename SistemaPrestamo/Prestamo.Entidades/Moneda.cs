using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prestamo.Entidades
{
    public class Moneda
    {
        public int IdMoneda { get; set; }
        public string Nombre { get; set; } = null!;
        public string Simbolo { get; set; } = null!;
        public string FechaCreacion { get; set; } = null!;
    }
}
