﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSGIO.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }        

        public ActionResult Portafolio()
        {
            return View();
        }
        public ActionResult PortafolioItem()
        {
            return View();
        }
    }
}