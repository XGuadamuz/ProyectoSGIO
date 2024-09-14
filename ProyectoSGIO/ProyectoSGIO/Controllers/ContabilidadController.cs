using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSGIO.Controllers
{
    public class ContabilidadController : Controller
    {
        public ActionResult RegistroFactura()
        {
            return View();
        }
        public ActionResult EditarFactura()
        {
            return View();
        }
        public ActionResult VisualizarFacturas()
        {
            return View();
        }
        public ActionResult DetalleFactura()
        {
            return View();
        }

        //Cierres Financieros
        public ActionResult CierreFinancieroMes()
        {
            return View();
        }
        public ActionResult ConfirmarCierreMes()
        {
            return View();
        }

        //Anual
        public ActionResult CierreFinancieroAnual()
        {
            return View();
        }
        public ActionResult ConfirmarCierreAnual()
        {
            return View();
        }
        public ActionResult ReconciliacionCuentas()
        {
            return View();
        }
    }
}