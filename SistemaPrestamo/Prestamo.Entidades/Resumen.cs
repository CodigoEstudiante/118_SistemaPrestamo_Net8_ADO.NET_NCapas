using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prestamo.Entidades
{
    public class Resumen
    {
        public string TotalClientes { get; set; } = null!;
        public string PrestamosPendientes { get; set; } = null!;
        public string PrestamosCancelados { get; set; } = null!;
        public string InteresAcumulado { get; set; } = null!;
    }
}
