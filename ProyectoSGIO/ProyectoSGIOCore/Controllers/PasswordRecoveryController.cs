using Microsoft.AspNetCore.Mvc;

namespace ProyectoSGIOCore.Controllers
{
    public class PasswordRecoveryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
