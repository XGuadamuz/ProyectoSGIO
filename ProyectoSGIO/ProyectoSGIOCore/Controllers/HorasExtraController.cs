using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using System.Linq;

namespace ProyectoSGIOCore.Controllers
{
    public class HorasExtraController : Controller
    {
        private readonly AppDBContext _context;

        public HorasExtraController(AppDBContext context)
        {
            _context = context;
        }

        // Registro de Horas Extra
        [HttpPost]
        public IActionResult RegistrarHorasExtra(int empleadoId, DateTime fecha, double cantidadHoras)
        {
            var horaExtra = new HoraExtra
            {
                IdEmpleado = empleadoId,
                Fecha = fecha,
                CantidadHoras = cantidadHoras,
                Aprobada = false
            };

            _context.HorasExtra.Add(horaExtra);
            _context.SaveChanges();

            return RedirectToAction("DetalleEmpleado", new { id = empleadoId });
        }

        public IActionResult VerHorasExtra(int empleadoId)
        {
            var horasExtra = _context.HorasExtra
                .Where(h => h.IdEmpleado == empleadoId && h.Fecha >= DateTime.Now.AddMonths(-1))
                .ToList();

            return View(horasExtra);
        }

        [HttpPost]
        public IActionResult EditarHorasExtra(int idHoraExtra, double nuevaCantidadHoras)
        {
            var horaExtra = _context.HorasExtra.Find(idHoraExtra);

            if (horaExtra != null)
            {
                horaExtra.CantidadHoras = nuevaCantidadHoras;
                _context.SaveChanges();
            }

            return RedirectToAction("VerHorasExtra", new { empleadoId = horaExtra.IdEmpleado });
        }

        [HttpPost]
        public IActionResult AprobarHorasExtra(int idHoraExtra, bool aprobar)
        {
            var horaExtra = _context.HorasExtra.Find(idHoraExtra);

            if (horaExtra != null)
            {
                horaExtra.Aprobada = aprobar;
                _context.SaveChanges();
            }

            // Notificar al empleado
            return RedirectToAction("VerHorasExtra", new { empleadoId = horaExtra.IdEmpleado });
        }
    }
}
