using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prestamo.Entidades
{
    public class Prestamo
    {
        public int IdPrestamo { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public Moneda Moneda { get; set; } = null!;
        public string FechaInicioPago { get; set; } = null!;
        public string MontoPrestamo { get; set; } = null!;
        public string InteresPorcentaje { get; set; } = null!;
        public int NroCuotas { get; set; }
        public string FormaDePago { get; set; } = null!;
        public string ValorPorCuota { get; set; } = null!;
        public string ValorInteres { get; set; } = null!;
        public string ValorTotal { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string FechaCreacion { get; set; } = null!;
        public List<PrestamoDetalle> PrestamoDetalle { get; set; } = null!;
    }
}
