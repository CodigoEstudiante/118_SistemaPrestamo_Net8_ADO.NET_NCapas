using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamo.Data;
using Prestamo.Entidades;

namespace Prestamo.Web.Controllers
{
    [Authorize]
    public class MonedaController : Controller
    {
        private readonly MonedaData _monedaData;
        public MonedaController(MonedaData monedaData)
        {
            _monedaData = monedaData;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Moneda> lista = await _monedaData.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Moneda objeto)
        {
            string respuesta = await _monedaData.Crear(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Moneda objeto)
        {
            string respuesta = await _monedaData.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int Id)
        {
            string respuesta = await _monedaData.Eliminar(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }
    }
}
