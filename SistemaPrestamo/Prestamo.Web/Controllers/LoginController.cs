using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Prestamo.Data;
using Prestamo.Entidades;
using System.Security.Claims;

namespace Prestamo.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioData _usuarioData;
        public LoginController(UsuarioData usuarioData)
        {
            _usuarioData = usuarioData;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string correo,string clave)
        {
            if (correo == null || clave == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            Usuario usuario_encontrado = new Usuario();
            usuario_encontrado = await _usuarioData.Obtener(correo,clave);

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            ViewData["Mensaje"] = null;

            //aqui guarderemos la informacion de nuestro usuario
            List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, usuario_encontrado.NombreCompleto),
                    new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Role,"Administrador")
                };


            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
            return RedirectToAction("Index", "Home");
        }
    }
}
