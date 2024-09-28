using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.Controllers
{
    public class LoginController : Controller
    {

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                // Falta lógica para crear el usuario y guardarlo en la base de datos
                // Redirigir a una vista de éxito o al inicio de sesión
                TempData["SuccessMessage"] = "Usuario creado exitosamente.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(login);
            }
        }
    }
}
