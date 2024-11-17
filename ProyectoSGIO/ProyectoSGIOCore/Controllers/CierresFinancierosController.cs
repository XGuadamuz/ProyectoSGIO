using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using System.Linq;

namespace ProyectoSGIOCore.Controllers
{
    public class CierresFinancierosController : Controller
    {
        private readonly AppDBContext _dbContext;

        public CierresFinancierosController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult RegistrarCierre()
        {
            return View("~/Views/Facturas/RegistrarCierre.cshtml");
        }

        [HttpPost]
        public IActionResult RegistrarCierre(int anio, string observaciones)
        {
            var ingresos = _dbContext.Facturas
                .Where(f => f.FechaEmision.Year == anio && f.MontoTotal > 0)
                .Sum(f => f.MontoTotal);

            var egresos = _dbContext.Facturas
                .Where(f => f.FechaEmision.Year == anio && f.MontoTotal < 0)
                .Sum(f => f.MontoTotal);

            var cierre = new CierreFinanciero
            {
                Anio = anio,
                FechaCierre = DateTime.Now,
                TotalIngresos = ingresos,
                TotalEgresos = Math.Abs(egresos), // Convertir a positivo
                Utilidad = ingresos - Math.Abs(egresos),
                Observaciones = observaciones
            };

            _dbContext.CierresFinancieros.Add(cierre);
            _dbContext.SaveChanges();

            return RedirectToAction("VisualizarCierres");
        }

        [HttpGet]
        public IActionResult VisualizarCierres()
        {
            // Obtiene la lista de cierres financieros
            var cierres = _dbContext.CierresFinancieros.ToList();

            return View("~/Views/Facturas/VisualizarCierres.cshtml", cierres);
        }
    }
}
