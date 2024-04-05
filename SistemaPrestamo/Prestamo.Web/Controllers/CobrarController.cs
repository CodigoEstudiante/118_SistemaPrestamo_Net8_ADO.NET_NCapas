using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamo.Data;

namespace Prestamo.Web.Controllers
{
    [Authorize]
    public class CobrarController : Controller
    {
        private readonly ClienteData _clienteData;
        private readonly PrestamoData _prestamoData;
        public CobrarController(ClienteData clienteData, PrestamoData prestamoData)
        {
            _clienteData = clienteData;
            _prestamoData = prestamoData;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PagarCuotas(int idPrestamo,string nroCuotasPagadas)
        {
            string respuesta = await _prestamoData.PagarCuotas(idPrestamo,nroCuotasPagadas);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }
    }
}
