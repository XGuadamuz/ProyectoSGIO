using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProyectoSGIOCore.Models;
using ProyectoSGIOCore.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using ProyectoSGIOCore.Data;

namespace ProyectoSGIOCore.Controllers
{
    public class CronogramaController : Controller
    {
        private readonly AppDBContext _context;

        public CronogramaController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Cronograma(int proyectoId)
        {
            var proyecto = await _context.Proyectos
                .Include(p => p.Fases)
                    .ThenInclude(f => f.Tareas)
                .FirstOrDefaultAsync(p => p.Id == proyectoId);

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Index");
            }

            var tareas = proyecto.Fases.SelectMany(f => f.Tareas).ToList();

            var dependencias = await _context.Dependencias
                .Where(d => tareas.Select(t => t.Id).Contains(d.TareaPredecesoraId) || tareas.Select(t => t.Id).Contains(d.TareaSucesoraId))
                .ToListAsync();

            var ganttData = tareas.Select(t => new
            {
                id = t.Id,
                name = t.Nombre,
                actualStart = t.FechaInicio.ToString("yyyy-MM-dd"),
                actualEnd = t.FechaFin.ToString("yyyy-MM-dd"),
                dependencies = dependencias.Where(d => d.TareaSucesoraId == t.Id).Select(d => new { id = d.TareaPredecesoraId }).ToList()
            }).ToList();

            ViewBag.GanttData = JsonConvert.SerializeObject(ganttData);

            var model = new CronogramaVM
            {
                Proyecto = proyecto,
                Tareas = tareas,
                Dependencias = dependencias
            };

            return View(model);
        }
    }
}

