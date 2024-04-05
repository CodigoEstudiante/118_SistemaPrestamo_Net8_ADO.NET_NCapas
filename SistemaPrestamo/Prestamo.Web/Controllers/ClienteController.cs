using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamo.Data;
using Prestamo.Entidades;

namespace Prestamo.Web.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ClienteData _clienteData;
        public ClienteController(ClienteData clienteData)
        {
            _clienteData = clienteData;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Cliente> lista = await _clienteData.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Cliente objeto)
        {
            string respuesta = await _clienteData.Crear(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Cliente objeto)
        {
            string respuesta = await _clienteData.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int Id)
        {
            string respuesta = await _clienteData.Eliminar(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }
    }
}
