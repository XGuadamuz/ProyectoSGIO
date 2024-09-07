using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSGIO.Controllers
{
    public class LoginController : Controller
    {       
        public ActionResult Registro()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult OlvidoContraseña()
        {
            return View();
        }
    }
}