using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Administrador , Supervisor")]
    public class ProyectoController : Controller
    {
        private readonly AppDBContext _dbContext;

        public ProyectoController(AppDBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> Proyectos()
        {
            var proyectos = await _dbContext.Proyectos.ToListAsync();
            return View(proyectos);
        }


        [HttpGet]
        public IActionResult CrearProyecto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearProyecto(Proyecto proyecto, List<Tarea> tareas)
        {
            if (string.IsNullOrEmpty(proyecto.Nombre))
            {
                TempData["MensajeError"] = "El nombre del proyecto no puede estar vacío.";
                return View(proyecto);
            }

            foreach (var tarea in tareas)
            {
                if (string.IsNullOrEmpty(tarea.Nombre))
                {
                    TempData["MensajeError"] = "El nombre de cada tarea no puede estar vacío.";
                    return View(proyecto);
                }

                if (tarea.FechaInicio >= tarea.FechaFin)
                {
                    TempData["MensajeError"] = "La fecha de inicio debe ser anterior a la fecha de finalización para cada tarea.";
                    return View(proyecto);
                }
            }

            if (ModelState.IsValid)
            {
                proyecto.FechaCreacion = DateTime.Now;
                _dbContext.Proyectos.Add(proyecto);
                await _dbContext.SaveChangesAsync();

                foreach (var tarea in tareas)
                {
                    tarea.ProyectoId = proyecto.Id;
                    _dbContext.Tareas.Add(tarea);
                }

                await _dbContext.SaveChangesAsync();
                TempData["MensajeExito"] = "Proyecto y tareas creados correctamente.";
                return RedirectToAction("Proyectos");
            }

            TempData["MensajeError"] = "Ocurrió un error al crear el proyecto.";
            return View(proyecto);
        }

        [HttpGet]
        public async Task<IActionResult> AgregarTareas(int id)
        {
            var proyecto = await _dbContext.Proyectos.Include(p => p.Tareas)
                                                   .FirstOrDefaultAsync(p => p.Id == id);
            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }
            return View(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTareas(int id, List<Tarea> tareas)
        {
            var proyecto = await _dbContext.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }

            foreach (var tarea in tareas)
            {
                if (string.IsNullOrEmpty(tarea.Nombre))
                {
                    TempData["MensajeError"] = "El nombre de cada tarea no puede estar vacío.";
                    return RedirectToAction("AgregarTareas", new { id });
                }

                if (tarea.FechaInicio >= tarea.FechaFin)
                {
                    TempData["MensajeError"] = "La fecha de inicio debe ser anterior a la fecha de finalización para cada tarea.";
                    return RedirectToAction("AgregarTareas", new { id });
                }

                tarea.ProyectoId = id;
                _dbContext.Tareas.Add(tarea);
            }

            await _dbContext.SaveChangesAsync();
            TempData["MensajeExito"] = "Tareas agregadas correctamente.";
            return RedirectToAction("Detalles", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> GestionarProyecto(int id)
        {
            var proyecto = await _dbContext.Proyectos.Include(p => p.Tareas)
                                                   .FirstOrDefaultAsync(p => p.Id == id);
            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }
            return View(proyecto);
        }
    }
}
