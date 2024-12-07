using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using ProyectoSGIOCore.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;
using ProyectoSGIOCore.Services;
using System.Text;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Administrador, Supervisor")]
    public class ProyectoController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly IUtilitariosModel _utilitarios;

        public ProyectoController(AppDBContext context, IUtilitariosModel utilitarios)
        {
            _dbContext = context;
            _utilitarios = utilitarios;
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

        [HttpGet]
        public async Task<IActionResult> Dashboard(int id)
        {
            var proyecto = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .Include(p => p.Hitos)
                .ThenInclude(h => h.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proyecto == null) return NotFound();

            var usuarios = await _dbContext.Usuarios
                .Where(u => u.Rol.Nombre == "Empleado")
                .ToListAsync();

            var estadosHitos = new List<EstadoHitoVM>
    {
        new EstadoHitoVM { Id = 1, Nombre = "Completo" },
        new EstadoHitoVM { Id = 2, Nombre = "Pendiente" },
        new EstadoHitoVM { Id = 3, Nombre = "En Progreso" }
    };

            var hitoData = proyecto.Hitos
                .GroupBy(h => h.estado)
                .Select(g => new
                {
                    Estado = g.Key,
                    Nombre = estadosHitos.FirstOrDefault(e => e.Id == g.Key)?.Nombre,
                    Count = g.Count(),
                    CountP = g.Count() * 10
                })
                .ToList();

            var tareaData = proyecto.Fases
                .SelectMany(f => f.Tareas)
                .GroupBy(t => t.Completada)
                .Select(g => new
                {
                    Completada = g.Key,
                    Nombre = g.Key ? "Completadas" : "No Completadas",
                    Count = g.Count(),
                    CountP = g.Count() *10
                })
                .ToList();

            var faseData = proyecto.Fases
                .Select(f => new
                {
                    f.Nombre,
                    PorcentajeCompletadas = f.Tareas.Count == 0
                        ? 0
                        : (f.Tareas.Count(t => t.Completada) * 100 / f.Tareas.Count())
                })
                .ToList();

            // Calcular el progreso total del proyecto
            var totalTareas = proyecto.Fases.SelectMany(f => f.Tareas).Count();
            var tareasCompletadas = proyecto.Fases.SelectMany(f => f.Tareas).Count(t => t.Completada);
            var progresoGeneral = totalTareas == 0 ? 0 : (tareasCompletadas * 100 / totalTareas);

            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Correo");
            ViewBag.ProyectoId = id;
            ViewBag.EstadosHito = new SelectList(estadosHitos, "Id", "Nombre");
            ViewBag.HitoData = hitoData;
            ViewBag.TareaData = tareaData;
            ViewBag.FaseData = faseData;
            ViewBag.HitoDataJson = JsonConvert.SerializeObject(hitoData);
            ViewBag.TareaDataJson = JsonConvert.SerializeObject(tareaData);
            ViewBag.FaseDataJson = JsonConvert.SerializeObject(faseData);
            ViewBag.ProgresoGeneral = progresoGeneral; // Pasar el progreso general a la vista

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> AsignarHito(int id)
        {
            var proyecto = await _dbContext.Proyectos.FindAsync(id);
            if (proyecto == null) return NotFound();

            var usuarios = await _dbContext.Usuarios
                .Where(u => u.Rol.Nombre == "Empleado")
                .ToListAsync();
            var estadosHitos = new List<EstadoHitoVM> 
            { new EstadoHitoVM { Id = 1, Nombre = "Completo" }, 
             new EstadoHitoVM { Id = 2, Nombre = "Pendiente" },
             new EstadoHitoVM { Id = 3, Nombre = "En Progreso" } };
            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Correo");
            ViewBag.ProyectoId = id;
            ViewBag.EstadosHito = new SelectList(estadosHitos, "Id","Nombre");

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

            if (ModelState.IsValid)
            {
                try
                {
                    proyecto.FechaCreacion = DateTime.Now;

                    // Comprobar si ya existe un proyecto con el mismo nombre (opcional)
                    var proyectoExistente = await _dbContext.Proyectos
                        .Include(p => p.Fases)
                        .ThenInclude(f => f.Tareas)
                        .FirstOrDefaultAsync(p => p.Nombre == proyecto.Nombre);

                    if (proyectoExistente != null)
                    {
                        TempData["MensajeError"] = "Ya existe un proyecto con ese nombre.";
                        return View(proyecto);
                    }

                    _dbContext.Proyectos.Add(proyecto);
                    await _dbContext.SaveChangesAsync();

                    var nombresFasesProcesadas = new HashSet<string>();

                    foreach (var fase in fases)
                    {
                        // Validar si la fase ya existe en el proyecto
                        bool faseExiste = await _dbContext.Fases
                            .AnyAsync(f => f.ProyectoId == proyecto.Id && f.Nombre == fase.Nombre);

                        if (faseExiste || nombresFasesProcesadas.Contains(fase.Nombre))
                        {
                            TempData["MensajeError"] = $"La fase '{fase.Nombre}' ya existe en este proyecto.";
                            continue;
                        }

                        nombresFasesProcesadas.Add(fase.Nombre);

                        fase.Id = 0;
                        fase.ProyectoId = proyecto.Id;
                        _dbContext.Fases.Add(fase);
                        await _dbContext.SaveChangesAsync();

                        foreach (var tarea in fase.Tareas)
                        {
                            tarea.Id = 0;
                            tarea.FaseId = fase.Id;
                            _dbContext.Tareas.Add(tarea);
                        }
                    }

                    await _dbContext.SaveChangesAsync();

                    // Guardar el plan inicial
                    var planInicial = new PlanInicial 
                    {
                        ProyectoId = proyecto.Id,
                        FasesIniciales = fases.Select(f => new FaseInicial 
                        {
                            Nombre = f.Nombre,
                            TareasIniciales = f.Tareas.Select(t => new TareaInicial 
                            {
                                Nombre = t.Nombre,
                                FechaInicio = t.FechaInicio,
                                FechaFin = t.FechaFin 
                            }).ToList()
                        }).ToList()
                    };

                    _dbContext.PlanesIniciales.Add(planInicial);
                    await _dbContext.SaveChangesAsync();

                    TempData["MensajeExito"] = $"Proyecto creado correctamente. Costo total: {proyecto.CostoTotal:C}";
                    return RedirectToAction("Proyectos");
                }
                catch (Exception ex)
                {
                    TempData["MensajeError"] = $"Ocurrió un error al crear el proyecto: {ex.Message}";
                    return View(proyecto);
                }
            }

            TempData["MensajeError"] = "El modelo no es válido. Por favor, verifica los datos ingresados.";
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

                    _dbContext.Hitos.RemoveRange(proyecto.Hitos);

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
        public async Task<IActionResult> AsignarHito(int id, int usuarioId, int estadoId, string Descripcion, DateTime Fecha)
        {
            var proyecto = await _dbContext.Proyectos.FindAsync(id);
            if (proyecto == null) return NotFound();

            // Verificar si se seleccionó un usuario
            if (usuarioId == 0)
            {
                TempData["MensajeError"] = "Debe seleccionar un empleado para asignar al proyecto.";
                ViewBag.ProyectoId = id;

                // Volver a cargar los usuarios para que se muestren en el dropdown
                var usuarios = await _dbContext.Usuarios
                    .Where(u => u.Rol.Nombre == "Usuario")
                    .ToListAsync();
                ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Correo");

                return View(proyecto);
            }

            var hito = new Hito();
            hito.Fecha = Fecha;
            hito.IdUsuario = usuarioId;
            hito.Descripcion = Descripcion;
            hito.ProyectoId = id;
            hito.estado = estadoId;

            _dbContext.Hitos.Add(hito);
            _dbContext.SaveChanges();

            TempData["MensajeExito"] = "Hito asignado correctamente.";
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


        [HttpPost]
        public async Task<IActionResult> EliminarHito(int hitoId)
        {
            var hito = await _dbContext.Hitos
                .FirstOrDefaultAsync(h => h.ID == hitoId);

            if (hito == null)
            {
                TempData["MensajeError"] = "El Hito no fue encontrado.";
                return RedirectToAction("Proyectos");
            }

            try
            {
                _dbContext.Hitos.Remove(hito);

                await _dbContext.SaveChangesAsync();
                TempData["MensajeExito"] = "Hito eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al eliminar el hito: {ex.Message}";
            }

            return RedirectToAction("GestionarProyecto", new { id = hito.ProyectoId });
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
            return Json(new { exito = true, mensaje = "Tarea agregada correctamente." });
        }

        [HttpPost]
        public IActionResult EliminarTarea(int tareaId)
        {
            try
            {
                var tarea = _dbContext.Tareas
                    .Include(t => t.Fase)
                    .FirstOrDefault(t => t.Id == tareaId);

                if (tarea == null)
                {
                    TempData["MensajeError"] = "No se encontró la tarea a eliminar.";
                    return RedirectToAction("GestionarProyecto");
                }

                if (tarea.Fase == null)
                {
                    TempData["MensajeError"] = "La tarea no está asociada a una fase válida.";
                    return RedirectToAction("GestionarProyecto");
                }

                int proyectoId = tarea.Fase.ProyectoId;

                // Eliminar la tarea
                _dbContext.Tareas.Remove(tarea);
                _dbContext.SaveChanges();

                TempData["MensajeExito"] = $"La tarea '{tarea.Nombre}' se eliminó correctamente.";

                // Redirigir a la vista de gestión del proyecto
                return RedirectToAction("GestionarProyecto", new { id = proyectoId });
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Ocurrió un error al intentar eliminar la tarea.";
                Console.WriteLine(ex.Message);
                return RedirectToAction("GestionarProyecto");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GestionarProyecto(int id)
        {
            var proyecto = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .Include(h => h.Hitos)
                .ThenInclude(u=>u.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
            var usuarios = await _dbContext.Usuarios
                .Where(u => u.Rol.Nombre == "Empleado")
                .ToListAsync();
            var estadosHitos = new List<EstadoHitoVM>
            { new EstadoHitoVM { Id = 1, Nombre = "Completo" },
             new EstadoHitoVM { Id = 2, Nombre = "Pendiente" },
             new EstadoHitoVM { Id = 3, Nombre = "En Progreso" } };
            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Correo");
            ViewBag.ProyectoId = id;
            ViewBag.EstadosHito = new SelectList(estadosHitos, "Id", "Nombre");

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }

            return View(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios(int proyectoId, EstadoProyecto Estado, List<int> tareasCompletadas)
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
                proyecto.Estado = Estado;

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

        [HttpGet]
        public async Task<IActionResult> DashboardComparacion(int proyectoId)
        {
            var proyecto = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .Include(p => p.Hitos)
                .ThenInclude(h => h.Usuario)
                .FirstOrDefaultAsync(p => p.Id == proyectoId);

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }

            var planInicial = await _dbContext.PlanesIniciales
                .Include(p => p.FasesIniciales)
                .ThenInclude(f => f.TareasIniciales)
                .FirstOrDefaultAsync(p => p.ProyectoId == proyectoId);

            var progresoActual = proyecto.Fases.Select(f => new
            {
                f.Nombre,
                Tareas = f.Tareas.Select(t => new
                {
                    t.Nombre,
                    t.FechaInicio,
                    t.FechaFin,
                    t.Completada
                }).ToList()
            }).ToList();

            var desviaciones = new List<dynamic>();
            var notificacionRequerida = false;
            var mensajeDesviaciones = "<h3>Desviaciones Significativas Detectadas:</h3><ul>";

            foreach (var fase in planInicial.FasesIniciales)
            {
                var faseActual = progresoActual.FirstOrDefault(f => f.Nombre == fase.Nombre);
                if (faseActual != null)
                {
                    var tareasInicial = fase.TareasIniciales.Count;
                    var tareasCompletadas = faseActual.Tareas.Count(t => t.Completada);
                    var desviacion = (double)tareasCompletadas / tareasInicial - 1;

                    if (Math.Abs(desviacion) >= 0.5)
                    {
                        desviaciones.Add(new
                        {
                            Fase = fase.Nombre,
                            Desviacion = desviacion
                        });

                        notificacionRequerida = true;
                        mensajeDesviaciones += $"<li>Fase: {fase.Nombre}, Desviación: {desviacion:P2}</li>";
                    }
                }
            }

            mensajeDesviaciones += "</ul>";

            if (notificacionRequerida)
            {
                var correoSupervisor = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
                _utilitarios.EnviarCorreo(correoSupervisor, "Desviaciones Significativas Detectadas", mensajeDesviaciones);
            }

            ViewBag.FaseNombres = JsonConvert.SerializeObject(planInicial.FasesIniciales.Select(f => f.Nombre).ToList());
            ViewBag.ProgresoInicial = JsonConvert.SerializeObject(planInicial.FasesIniciales.Select(f => f.TareasIniciales.Count).ToList());
            ViewBag.ProgresoActual = JsonConvert.SerializeObject(progresoActual.Select(f => f.Tareas.Count(t => t.Completada)).ToList());
            ViewBag.Desviaciones = desviaciones.ToList();

            var model = new DashboardComparacionVM
            {
                Proyecto = proyecto,
                PlanInicial = planInicial,
                ProgresoActual = progresoActual
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MedidasCorrectivas(int proyectoId)
        {
            var proyecto = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .Include(p => p.Hitos)
                .ThenInclude(h => h.Usuario)
                .FirstOrDefaultAsync(p => p.Id == proyectoId);

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }

            var planInicial = await _dbContext.PlanesIniciales
                .Include(p => p.FasesIniciales)
                .ThenInclude(f => f.TareasIniciales)
                .FirstOrDefaultAsync(p => p.ProyectoId == proyectoId);

            var progresoActual = proyecto.Fases.Select(f => new
            {
                f.Nombre,
                Tareas = f.Tareas.Select(t => new
                {
                    t.Nombre,
                    t.FechaInicio,
                    t.FechaFin,
                    t.Completada
                }).ToList()
            }).ToList();

            var medidasCorrectivas = new List<dynamic>();

            foreach (var fase in planInicial.FasesIniciales)
            {
                var faseActual = progresoActual.FirstOrDefault(f => f.Nombre == fase.Nombre);
                if (faseActual != null)
                {
                    var tareasInicial = fase.TareasIniciales.Count;
                    var tareasCompletadas = faseActual.Tareas.Count(t => t.Completada);
                    var desviacion = (double)tareasCompletadas / tareasInicial - 1;

                    if (Math.Abs(desviacion) >= 0.5)
                    {
                        medidasCorrectivas.Add(new
                        {
                            Fase = fase.Nombre,
                            Medida = "Aumentar recursos para completar tareas pendientes.",
                            Impacto = "Se espera reducir la desviación y completar el 100% de las tareas planificadas."
                        });
                    }
                }
            }

            ViewBag.MedidasCorrectivas = medidasCorrectivas;

            var model = new DashboardComparacionVM
            {
                Proyecto = proyecto,
                PlanInicial = planInicial,
                ProgresoActual = progresoActual
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AprobarMedidasCorrectivas(int proyectoId, string[] medidasAprobadas)
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

            foreach (var faseNombre in medidasAprobadas)
            {
                var fase = proyecto.Fases.FirstOrDefault(f => f.Nombre == faseNombre);
                if (fase != null)
                {
                    // Implementar la medida correctiva
                    // Ejemplo: Añadir un recurso adicional a las tareas pendientes
                    foreach (var tarea in fase.Tareas.Where(t => !t.Completada))
                    {
                        // Añadir lógica específica para implementar la medida correctiva
                        // Ejemplo: Aumentar recursos, ajustar fechas, etc.
                        tarea.FechaFin = tarea.FechaFin.AddDays(-7); // Acortar la fecha de finalización como medida correctiva
                    }

                    // Registrar el impacto de la medida correctiva
                    var impacto = new ImpactoMedidaCorrectiva
                    {
                        ProyectoId = proyecto.Id,
                        Fase = fase.Nombre,
                        Medida = "Aumentar recursos para completar tareas pendientes.",
                        FechaImplementacion = DateTime.Now,
                        Impacto = "Se espera reducir la desviación y completar el 100% de las tareas planificadas."
                    };

                    _dbContext.ImpactosMedidasCorrectivas.Add(impacto);
                }
            }

            await _dbContext.SaveChangesAsync();

            TempData["MensajeExito"] = "Medidas correctivas aprobadas e implementadas correctamente.";
            return RedirectToAction("DashboardComparacion", new { proyectoId = proyecto.Id });
        }

        [HttpGet]
        public async Task<IActionResult> SeguimientoImpacto(int proyectoId)
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

            var impactos = await _dbContext.ImpactosMedidasCorrectivas
                .Where(i => i.ProyectoId == proyectoId)
                .ToListAsync();

            ViewBag.ProyectoId = proyectoId;
            return View(impactos);
        }

        [HttpGet]
        public async Task<IActionResult> DetectarCambiosSignificativos(int proyectoId)
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

            var planInicial = await _dbContext.PlanesIniciales
                .Include(p => p.FasesIniciales)
                .ThenInclude(f => f.TareasIniciales)
                .FirstOrDefaultAsync(p => p.ProyectoId == proyectoId);

            var cambiosSignificativos = false;
            var mensajeCambios = "<h3>Cambios Significativos Detectados:</h3><ul>";

            foreach (var fase in planInicial.FasesIniciales)
            {
                var faseActual = proyecto.Fases.FirstOrDefault(f => f.Nombre == fase.Nombre);
                if (faseActual != null)
                {
                    var tareasInicial = fase.TareasIniciales.Count;
                    var tareasActuales = faseActual.Tareas.Count;
                    var cambio = Math.Abs(tareasActuales - tareasInicial) > 0.5 * tareasInicial;

                    if (cambio)
                    {
                        cambiosSignificativos = true;
                        mensajeCambios += $"<li>Fase: {fase.Nombre}, Cambio en Número de Tareas: de {tareasInicial} a {tareasActuales}</li>";
                    }
                }
            }

            mensajeCambios += "</ul>";

            ViewBag.CambiosSignificativos = cambiosSignificativos;
            ViewBag.MensajeCambios = mensajeCambios;

            var model = new DashboardComparacionVM
            {
                Proyecto = proyecto,
                PlanInicial = planInicial,
                ProgresoActual = proyecto.Fases.Select(f => new
                {
                    f.Nombre,
                    Tareas = f.Tareas.Select(t => new
                    {
                        t.Nombre,
                        t.FechaInicio,
                        t.FechaFin,
                        t.Completada
                    }).ToList()
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarLineaBase(int proyectoId)
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

            // Actualizar la línea base
            var planInicial = await _dbContext.PlanesIniciales
                .Include(p => p.FasesIniciales)
                .ThenInclude(f => f.TareasIniciales)
                .FirstOrDefaultAsync(p => p.ProyectoId == proyectoId);

            planInicial.FasesIniciales.Clear();

            foreach (var fase in proyecto.Fases)
            {
                var faseInicial = new FaseInicial
                {
                    Nombre = fase.Nombre,
                    TareasIniciales = fase.Tareas.Select(t => new TareaInicial
                    {
                        Nombre = t.Nombre,
                        FechaInicio = t.FechaInicio,
                        FechaFin = t.FechaFin
                    }).ToList()
                };

                planInicial.FasesIniciales.Add(faseInicial);
            }

            await _dbContext.SaveChangesAsync();

            TempData["MensajeExito"] = "Línea base del proyecto actualizada correctamente.";
            return RedirectToAction("DashboardComparacion", new { proyectoId = proyecto.Id });
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPuntoControl(int proyectoId)
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

            var puntoControl = new PuntoControl
            {
                ProyectoId = proyecto.Id,
                FechaRegistro = DateTime.Now,
                FasesControl = proyecto.Fases.Select(f => new FaseControl
                {
                    Nombre = f.Nombre,
                    TareasControl = f.Tareas.Select(t => new TareaControl
                    {
                        Nombre = t.Nombre,
                        FechaInicio = t.FechaInicio,
                        FechaFin = t.FechaFin,
                        Completada = t.Completada
                    }).ToList()
                }).ToList()
            };

            _dbContext.PuntosControl.Add(puntoControl);
            await _dbContext.SaveChangesAsync();

            TempData["MensajeExito"] = "Punto de control registrado correctamente.";
            return RedirectToAction("DashboardComparacion", new { proyectoId = proyecto.Id });
        }

        [HttpGet]
        public async Task<IActionResult> CompararPuntosControl(int proyectoId)
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

            var puntosControl = await _dbContext.PuntosControl
                .Where(pc => pc.ProyectoId == proyectoId)
                .Include(pc => pc.FasesControl)
                .ThenInclude(fc => fc.TareasControl)
                .ToListAsync();

            var model = new ComparacionPuntosControlVM
            {
                Proyecto = proyecto,
                PuntosControl = puntosControl
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EvaluacionPeriodica(int proyectoId)
        {
            var proyecto = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .Include(p => p.Hitos)
                .ThenInclude(h => h.Usuario)
                .FirstOrDefaultAsync(p => p.Id == proyectoId);

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }

            var planInicial = await _dbContext.PlanesIniciales
                .Include(p => p.FasesIniciales)
                .ThenInclude(f => f.TareasIniciales)
                .FirstOrDefaultAsync(p => p.ProyectoId == proyectoId);

            var impactoMedidas = await _dbContext.ImpactosMedidasCorrectivas
                .Where(im => im.ProyectoId == proyectoId)
                .ToListAsync();

            var desviaciones = new List<dynamic>();

            foreach (var fase in planInicial.FasesIniciales)
            {
                var faseActual = proyecto.Fases.FirstOrDefault(f => f.Nombre == fase.Nombre);
                if (faseActual != null)
                {
                    var tareasInicial = fase.TareasIniciales.Count;
                    var tareasCompletadas = faseActual.Tareas.Count(t => t.Completada);
                    var desviacion = (double)tareasCompletadas / tareasInicial - 1;

                    desviaciones.Add(new
                    {
                        Fase = fase.Nombre,
                        Desviacion = desviacion,
                        TareasInicial = tareasInicial,
                        TareasCompletadas = tareasCompletadas
                    });
                }
            }

            var model = new EvaluacionPeriodicaVM
            {
                Proyecto = proyecto,
                PlanInicial = planInicial,
                Desviaciones = desviaciones,
                ImpactoMedidas = impactoMedidas
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GenerarInforme(int proyectoId)
        {
            // Obtener los datos necesarios para la evaluación periódica
            var proyecto = await _dbContext.Proyectos
                .Include(p => p.Fases)
                .ThenInclude(f => f.Tareas)
                .Include(p => p.Hitos)
                .ThenInclude(h => h.Usuario)
                .FirstOrDefaultAsync(p => p.Id == proyectoId);

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Proyectos");
            }

            var planInicial = await _dbContext.PlanesIniciales
                .Include(p => p.FasesIniciales)
                .ThenInclude(f => f.TareasIniciales)
                .FirstOrDefaultAsync(p => p.ProyectoId == proyectoId);

            var impactoMedidas = await _dbContext.ImpactosMedidasCorrectivas
                .Where(im => im.ProyectoId == proyectoId)
                .ToListAsync();

            var desviaciones = new List<dynamic>();

            foreach (var fase in planInicial.FasesIniciales)
            {
                var faseActual = proyecto.Fases.FirstOrDefault(f => f.Nombre == fase.Nombre);
                if (faseActual != null)
                {
                    var tareasInicial = fase.TareasIniciales.Count;
                    var tareasCompletadas = faseActual.Tareas.Count(t => t.Completada);
                    var desviacion = (double)tareasCompletadas / tareasInicial - 1;

                    desviaciones.Add(new
                    {
                        Fase = fase.Nombre,
                        Desviacion = desviacion,
                        TareasInicial = tareasInicial,
                        TareasCompletadas = tareasCompletadas
                    });
                }
            }

            var model = new EvaluacionPeriodicaVM
            {
                Proyecto = proyecto,
                PlanInicial = planInicial,
                Desviaciones = desviaciones,
                ImpactoMedidas = impactoMedidas
            };

            // Generar el informe (esto puede ser en HTML, PDF, etc.)
            var informeHtml = new StringBuilder();
            informeHtml.Append("<h1>Informe de Evaluación Periódica</h1>");
            informeHtml.Append($"<h2>Proyecto: {model.Proyecto.Nombre}</h2>");
            informeHtml.Append("<h3>Desviaciones</h3>");
            informeHtml.Append("<ul>");
            foreach (var desviacion in model.Desviaciones)
            {
                informeHtml.Append($"<li>Fase: {desviacion.Fase}, Desviación: {desviacion.Desviacion:P2}, Tareas Iniciales: {desviacion.TareasInicial}, Tareas Completadas: {desviacion.TareasCompletadas}</li>");
            }
            informeHtml.Append("</ul>");
            informeHtml.Append("<h3>Impacto de las Medidas Correctivas</h3>");
            informeHtml.Append("<ul>");
            foreach (var impacto in model.ImpactoMedidas)
            {
                informeHtml.Append($"<li>Fase: {impacto.Fase}, Medida: {impacto.Medida}, Fecha de Implementación: {impacto.FechaImplementacion.ToShortDateString()}, Impacto: {impacto.Impacto}</li>");
            }
            informeHtml.Append("</ul>");

            return Content(informeHtml.ToString(), "text/html");
        }
    }
}
