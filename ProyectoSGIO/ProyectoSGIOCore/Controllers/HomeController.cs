using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoSGIOCore.Controllers
{
    [ResponseCache(Duration = 0, NoStore = true)]
    public class HomeController() : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Portafolio()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult PortafolioItem()
        {
            return View();
        }

    }
}
