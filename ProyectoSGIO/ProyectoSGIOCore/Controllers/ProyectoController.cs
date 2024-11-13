using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Administrador, Supervisor")]
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
            var proyectos = await _dbContext.Proyectos.Include(p => p.Fases).ThenInclude(f => f.Tareas).ToListAsync();
            return View(proyectos);
        }

        [HttpGet]
        public IActionResult CrearProyecto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearProyecto(Proyecto proyecto, List<Fase> fases)
        {
            if (string.IsNullOrEmpty(proyecto.Nombre))
            {
                TempData["MensajeError"] = "El nombre del proyecto no puede estar vacío.";
                return View(proyecto);
            }

            foreach (var fase in fases)
            {
                if (string.IsNullOrEmpty(fase.Nombre))
                {
                    TempData["MensajeError"] = "Cada fase debe tener un nombre.";
                    return View(proyecto);
                }

                foreach (var tarea in fase.Tareas)
                {
                    if (string.IsNullOrEmpty(tarea.Nombre))
                    {
                        TempData["MensajeError"] = "Cada tarea debe tener un nombre.";
                        return View(proyecto);
                    }

                    if (tarea.FechaInicio >= tarea.FechaFin)
                    {
                        TempData["MensajeError"] = "La fecha de inicio de cada tarea debe ser anterior a su fecha de finalización.";
                        return View(proyecto);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                proyecto.FechaCreacion = DateTime.Now;
                _dbContext.Proyectos.Add(proyecto);
                await _dbContext.SaveChangesAsync();

                foreach (var fase in fases)
                {
                    if (!_dbContext.Fases.Any(f => f.Nombre == fase.Nombre && f.ProyectoId == proyecto.Id))
                    {
                        fase.Id = 0;
                        fase.ProyectoId = proyecto.Id;
                        _dbContext.Fases.Add(fase);

                        foreach (var tarea in fase.Tareas)
                        {
                            if (!_dbContext.Tareas.Any(t => t.Nombre == tarea.Nombre && t.FaseId == fase.Id))
                            {
                                tarea.Id = 0;
                                tarea.FaseId = fase.Id;
                                _dbContext.Tareas.Add(tarea);
                            }
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
                TempData["MensajeExito"] = "Proyecto creado correctamente.";
                return RedirectToAction("Proyectos");
            }

            TempData["MensajeError"] = "Ocurrió un error al crear el proyecto.";
            return View(proyecto);
        }

        [HttpGet]
        public async Task<IActionResult> AgregarTareas(int faseId)
        {
            var fase = await _dbContext.Fases.Include(f => f.Tareas).FirstOrDefaultAsync(f => f.Id == faseId);
            if (fase == null)
            {
                TempData["MensajeError"] = "Fase no encontrada.";
                return RedirectToAction("Proyectos");
            }
            return View(fase);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTareas(int faseId, List<Tarea> tareas)
        {
            var fase = await _dbContext.Fases.Include(f => f.Tareas).FirstOrDefaultAsync(f => f.Id == faseId);
            if (fase == null)
            {
                TempData["MensajeError"] = "Fase no encontrada.";
                return RedirectToAction("Proyectos");
            }

            foreach (var tarea in tareas)
            {
                if (string.IsNullOrEmpty(tarea.Nombre))
                {
                    TempData["MensajeError"] = "El nombre de cada tarea no puede estar vacío.";
                    return RedirectToAction("AgregarTareas", new { faseId });
                }

                if (tarea.FechaInicio >= tarea.FechaFin)
                {
                    TempData["MensajeError"] = "La fecha de inicio debe ser anterior a la fecha de finalización para cada tarea.";
                    return RedirectToAction("AgregarTareas", new { faseId });
                }

                tarea.FaseId = faseId;
                _dbContext.Tareas.Add(tarea);
            }

            await _dbContext.SaveChangesAsync();
            TempData["MensajeExito"] = "Tareas agregadas correctamente.";
            return RedirectToAction("GestionarProyecto", new { id = fase.ProyectoId });
        }

        [HttpGet]
        public async Task<IActionResult> GestionarProyecto(int id)
        {
            var proyecto = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }

            return View(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios(int proyectoId, List<int> tareasCompletadas)
        {
            var proyecto = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .FirstOrDefaultAsync(p => p.Id == proyectoId);

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }

            try
            {
                foreach (var fase in proyecto.Fases)
                {
                    foreach (var tarea in fase.Tareas)
                    {
                        tarea.Completada = tareasCompletadas.Contains(tarea.Id);
                    }
                }

                await _dbContext.SaveChangesAsync();
                TempData["MensajeExito"] = "Cambios guardados exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al guardar los cambios: {ex.Message}";
            }

            return RedirectToAction("GestionarProyecto", new { id = proyectoId });
        }

        [HttpPost]
        public IActionResult ActualizarTareas([FromBody] Dictionary<int, bool> tareasCompletadas)
        {
            try
            {
                foreach (var tareaId in tareasCompletadas.Keys)
                {
                    var tarea = _dbContext.Tareas.Find(tareaId);
                    if (tarea != null)
                    {
                        tarea.Completada = tareasCompletadas[tareaId];
                    }
                }
                _dbContext.SaveChanges();
                return Ok(new { message = "Cambios guardados exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error al guardar cambios: {ex.Message}" });
            }
        }
    }
}
