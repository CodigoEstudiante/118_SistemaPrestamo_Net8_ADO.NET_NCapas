

namespace Prestamo.Entidades
{
    public class PrestamoDetalle
    {
        public int IdPrestamoDetalle { get; set; }
        public string FechaPago { get; set; } = null!;
        public int NroCuota { get; set; }
        public string MontoCuota { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string FechaPagado { get; set; } = null!;
    }
}
