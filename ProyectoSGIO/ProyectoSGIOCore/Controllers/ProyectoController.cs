using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var proyectos = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .Include(p => p.Usuario)
                .ToListAsync();

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
                    TempData["MensajeError"] = "El nombre de la fase no puede estar vacío.";
                    return View(proyecto);
                }

                foreach (var tarea in fase.Tareas)
                {
                    if (string.IsNullOrEmpty(tarea.Nombre))
                    {
                        TempData["MensajeError"] = "El nombre de la tarea no puede estar vacío.";
                        return View(proyecto);
                    }

                    if (tarea.FechaInicio >= tarea.FechaFin)
                    {
                        TempData["MensajeError"] = "La fecha de fin de la tarea no puede ser menor o igual a la fecha de inicio.";
                        return View(proyecto);
                    }
                }
            }

            // Guardar el proyecto y sus fases/tareas si todo está correcto
            if (ModelState.IsValid)
            {
                proyecto.FechaCreacion = DateTime.Now;
                _dbContext.Proyectos.Add(proyecto);
                await _dbContext.SaveChangesAsync();

                // Guardar fases y tareas relacionadas
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

        [HttpPost]
        public IActionResult EliminarProyecto(int proyectoId)
        {
            try
            {
                var proyecto = _dbContext.Proyectos
                    .Include(p => p.Fases)
                    .ThenInclude(f => f.Tareas)
                    .FirstOrDefault(p => p.Id == proyectoId);

                if (proyecto != null)
                {
                    // Eliminar tareas
                    foreach (var fase in proyecto.Fases)
                    {
                        _dbContext.Tareas.RemoveRange(fase.Tareas);
                    }

                    // Eliminar fases
                    _dbContext.Fases.RemoveRange(proyecto.Fases);

                    // Eliminar proyecto
                    _dbContext.Proyectos.Remove(proyecto);
                    _dbContext.SaveChanges();

                    TempData["MensajeExito"] = "El proyecto ha sido eliminado exitosamente.";
                }
                else
                {
                    TempData["MensajeError"] = "El proyecto no fue encontrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Ocurrió un error al eliminar el proyecto. Intente nuevamente.";
                // Log del error
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Proyectos");
        }

        [HttpGet]
        public async Task<IActionResult> AsignarCliente(int id)
        {
            var proyecto = await _dbContext.Proyectos.FindAsync(id);
            if (proyecto == null) return NotFound();

            var usuarios = await _dbContext.Usuarios
                .Where(u => u.Rol.Nombre == "Usuario")
                .ToListAsync();

            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Correo");
            ViewBag.ProyectoId = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AsignarCliente(int id, int usuarioId)
        {
            var proyecto = await _dbContext.Proyectos.FindAsync(id);
            if (proyecto == null) return NotFound();

            // Verificar si se seleccionó un usuario
            if (usuarioId == 0)
            {
                TempData["MensajeError"] = "Debe seleccionar un usuario para asignar al proyecto.";
                ViewBag.ProyectoId = id;

                // Volver a cargar los usuarios para que se muestren en el dropdown
                var usuarios = await _dbContext.Usuarios
                    .Where(u => u.Rol.Nombre == "Usuario")
                    .ToListAsync();
                ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Correo");

                return View(proyecto);
            }

            proyecto.IdUsuario = usuarioId;
            await _dbContext.SaveChangesAsync();

            TempData["MensajeExito"] = "Cliente asignado correctamente.";
            return RedirectToAction("Proyectos");
        }

        [HttpPost]
        public IActionResult AgregarFase(int proyectoId, string Nombre)
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                TempData["MensajeError"] = "El nombre de la fase no puede estar vacío.";
                return RedirectToAction("GestionarProyecto", new { id = proyectoId });
            }

            // Buscar el proyecto correspondiente
            var proyecto = _dbContext.Proyectos.Include(p => p.Fases).FirstOrDefault(p => p.Id == proyectoId);
            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("GestionarProyecto", new { id = proyectoId });
            }

            // Verificar si el nombre de la fase ya existe
            if (proyecto.Fases.Any(f => f.Nombre == Nombre))
            {
                TempData["MensajeError"] = "Ya existe una fase con este nombre en el proyecto.";
                return RedirectToAction("GestionarProyecto", new { id = proyectoId });
            }

            var nuevaFase = new Fase
            {
                Nombre = Nombre,
                ProyectoId = proyectoId,
                Tareas = new List<Tarea>()
            };

            // Guardar en la base de datos
            _dbContext.Fases.Add(nuevaFase);
            _dbContext.SaveChanges();

            TempData["MensajeExito"] = "Fase agregada correctamente.";
            return RedirectToAction("GestionarProyecto", new { id = proyectoId });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarFase(int faseId)
        {
            var fase = await _dbContext.Fases
                .Include(f => f.Tareas)
                .FirstOrDefaultAsync(f => f.Id == faseId);

            if (fase == null)
            {
                TempData["MensajeError"] = "La fase no fue encontrada.";
                return RedirectToAction("Proyectos");
            }

            try
            {
                // Eliminar todas las tareas asociadas a la fase
                _dbContext.Tareas.RemoveRange(fase.Tareas);

                // Eliminar la fase
                _dbContext.Fases.Remove(fase);

                await _dbContext.SaveChangesAsync();
                TempData["MensajeExito"] = "Fase eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al eliminar la fase: {ex.Message}";
            }

            return RedirectToAction("GestionarProyecto", new { id = fase.ProyectoId });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFase(int faseId)
        {
            var fase = await _dbContext.Fases.FirstOrDefaultAsync(f => f.Id == faseId);
            if (fase == null)
            {
                return NotFound(new { mensaje = "Fase no encontrada." });
            }
            return Json(new { id = fase.Id, nombre = fase.Nombre });
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTareasModal(int faseId, List<Tarea> tareas)
        {
            var fase = await _dbContext.Fases.Include(f => f.Tareas).FirstOrDefaultAsync(f => f.Id == faseId);
            if (fase == null)
            {
                return Json(new { exito = false, mensaje = "Fase no encontrada." });
            }

            foreach (var tarea in tareas)
            {
                if (string.IsNullOrEmpty(tarea.Nombre))
                {
                    return Json(new { exito = false, mensaje = "El nombre de cada tarea no puede estar vacío." });
                }

                if (tarea.FechaInicio >= tarea.FechaFin)
                {
                    return Json(new { exito = false, mensaje = "La fecha de inicio debe ser anterior a la fecha de finalización para cada tarea." });
                }

                tarea.FaseId = faseId;
                _dbContext.Tareas.Add(tarea);
            }

            await _dbContext.SaveChangesAsync();
            return Json(new { exito = true, mensaje = "Tareas agregadas correctamente." });
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
