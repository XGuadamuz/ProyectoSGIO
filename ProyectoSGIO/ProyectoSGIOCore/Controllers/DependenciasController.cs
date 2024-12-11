using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Models;
using ProyectoSGIOCore.Data;
using System.Threading.Tasks;
using ProyectoSGIOCore.Services;

namespace ProyectoSGIOCore.Controllers
{
    public class DependenciasController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IUtilitariosModel _utilitarios;

        public DependenciasController(AppDBContext context, IUtilitariosModel utilitarios)
        {
            _context = context;
            _utilitarios = utilitarios;
        }

        [HttpGet]
        public async Task<IActionResult> IndexDependencia()
        {
            var tareas = await _context.Tareas.ToListAsync();
            ViewBag.Tareas = tareas;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CrearDependencia()
        {
            var tareas = await _context.Tareas.ToListAsync();
            var fases = await _context.Fases.ToListAsync();
            ViewBag.Tareas = tareas;
            ViewBag.Fases = fases;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearDependencia(Tarea model, int DependenciaPredecesora, string TipoDependencia)
        {
            if (ModelState.IsValid)
            {
                // Verificar si la tarea predecesora está completada
                var tareaPredecesora = await _context.Tareas.FindAsync(DependenciaPredecesora);
                if (tareaPredecesora == null || tareaPredecesora.Completada || model.Completada)
                {
                    ViewBag.Message = "No se pueden establecer dependencias en tareas completadas.";
                    ViewBag.Success = false;
                    var todasTareas = await _context.Tareas.ToListAsync();
                    var todasFases = await _context.Fases.ToListAsync();
                    ViewBag.Tareas = todasTareas;
                    ViewBag.Fases = todasFases;
                    return View();
                }

                var nuevaTarea = new Tarea
                {
                    Nombre = model.Nombre,
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin,
                    Costo = model.Costo,
                    FaseId = model.FaseId
                };

                _context.Tareas.Add(nuevaTarea);
                await _context.SaveChangesAsync();

                if (DependenciaPredecesora != 0)
                {
                    var dependencia = new Dependencia
                    {
                        TareaPredecesoraId = DependenciaPredecesora,
                        TareaSucesoraId = nuevaTarea.Id,
                        TipoDependencia = TipoDependencia
                    };

                    _context.Dependencias.Add(dependencia);
                    await _context.SaveChangesAsync();

                    // Actualizar cronograma
                    var predecesora = await _context.Tareas.FindAsync(DependenciaPredecesora);
                    nuevaTarea.FechaInicio = predecesora.FechaFin.AddDays(1); // Asumimos que la sucesora empieza el día siguiente
                    _context.Tareas.Update(nuevaTarea);
                    await _context.SaveChangesAsync();

                    // Notificar cambios
                    NotificarCambio(dependencia, "creada");

                    ViewBag.Message = "Tarea creada exitosamente.";
                    ViewBag.Success = true;
                }
                else
                {
                    ViewBag.Message = "No se seleccionó ninguna dependencia predecesora.";
                    ViewBag.Success = false;
                }
            }
            else
            {
                ViewBag.Message = "Ocurrió un error al crear la tarea.";
                ViewBag.Success = false;
            }

            var todasTareasPost = await _context.Tareas.ToListAsync();
            var todasFasesPost = await _context.Fases.ToListAsync();
            ViewBag.Tareas = todasTareasPost;
            ViewBag.Fases = todasFasesPost;

            return View();
        }

        private void NotificarCambio(Dependencia dependencia, string accion)
        {
            var usuarios = _context.Usuarios.ToList();
            foreach (var usuario in usuarios)
            {
                var subject = $"Dependencia {accion} en el Proyecto";
                var message = $"Se ha {accion} una dependencia {dependencia.TipoDependencia} entre la tarea {dependencia.TareaPredecesoraId} y la tarea {dependencia.TareaSucesoraId}.";

                _utilitarios.EnviarCorreo(usuario.Correo, subject, message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> AñadirDatosDePrueba()
        {
            // Crear un proyecto de prueba
            var proyecto = new Proyecto
            {
                Nombre = "Proyecto de Prueba",
                FechaCreacion = DateTime.Now,
                Estado = EstadoProyecto.EnPlanificacion,
                IdUsuario = 1 // Asume que hay un usuario con ID 1
            };

            _context.Proyectos.Add(proyecto);
            await _context.SaveChangesAsync();

            // Crear una fase y asignarla al proyecto de prueba
            var fase = new Fase
            {
                Nombre = "Fase de Prueba",
                ProyectoId = proyecto.Id
            };

            _context.Fases.Add(fase);
            await _context.SaveChangesAsync();

            // Crear tareas y asignarlas a la fase de prueba
            var tarea1 = new Tarea
            {
                Nombre = "Tarea de Prueba 1",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(5),
                FaseId = fase.Id,
                Completada = false,
                Costo = 100
            };

            var tarea2 = new Tarea
            {
                Nombre = "Tarea de Prueba 2",
                FechaInicio = DateTime.Now.AddDays(6),
                FechaFin = DateTime.Now.AddDays(10),
                FaseId = fase.Id,
                Completada = false,
                Costo = 200
            };

            _context.Tareas.Add(tarea1);
            _context.Tareas.Add(tarea2);
            await _context.SaveChangesAsync();

            return Ok("Datos de prueba añadidos correctamente.");
        }

        [HttpGet]
        public async Task<IActionResult> AñadirPlanInicialDePrueba(int proyectoId)
        {
            // Verifica si el proyecto existe
            var proyecto = await _context.Proyectos.FindAsync(proyectoId);
            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            // Añadir Plan Inicial
            var planInicial = new PlanInicial
            {
                ProyectoId = proyectoId
            };

            _context.PlanesIniciales.Add(planInicial);
            await _context.SaveChangesAsync();

            // Añadir Fases Iniciales
            var faseInicial = new FaseInicial
            {
                Nombre = "Fase Inicial de Prueba",
                PlanInicialId = planInicial.Id
            };

            _context.FasesIniciales.Add(faseInicial);
            await _context.SaveChangesAsync();

            // Añadir Tareas Iniciales
            var tareaInicial = new TareaInicial
            {
                Nombre = "Tarea Inicial de Prueba",
                FaseInicialId = faseInicial.Id
            };

            _context.TareasIniciales.Add(tareaInicial);
            await _context.SaveChangesAsync();

            return Ok("Plan inicial de prueba añadido correctamente.");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarDependencia(int dependenciaId)
        {
            var dependencia = await _context.Dependencias.FindAsync(dependenciaId);
            if (dependencia == null)
            {
                ViewBag.Message = "Dependencia no encontrada.";
                ViewBag.Success = false;
                var dependencias = await _context.Dependencias.ToListAsync();
                ViewBag.Dependencias = dependencias;
                return View();
            }

            _context.Dependencias.Remove(dependencia);
            await _context.SaveChangesAsync();

            // Actualizar cronograma según las nuevas dependencias
            var sucesora = await _context.Tareas.FindAsync(dependencia.TareaSucesoraId);
            var predecesora = await _context.Tareas.FindAsync(dependencia.TareaPredecesoraId);

            if (sucesora != null && predecesora != null)
            {
                // Ajustar la fecha de inicio de la tarea sucesora
                sucesora.FechaInicio = predecesora.FechaFin.AddDays(1);
                _context.Tareas.Update(sucesora);
                await _context.SaveChangesAsync();

                // Recalcular el cronograma de todas las tareas sucesoras
                await RecalcularCronograma(sucesora);
            }

            // Notificar cambios
            NotificarCambio(dependencia, "eliminada");

            ViewBag.Message = "Dependencia eliminada exitosamente.";
            ViewBag.Success = true;

            var dependenciasActualizadas = await _context.Dependencias.ToListAsync();
            ViewBag.Dependencias = dependenciasActualizadas;

            return View();
        }

        private async Task RecalcularCronograma(Tarea tareaInicial)
        {
            var tareasRecalculadas = new List<Tarea> { tareaInicial };

            while (tareasRecalculadas.Count > 0)
            {
                var tareaActual = tareasRecalculadas.First();
                tareasRecalculadas.RemoveAt(0);

                var sucesoras = await _context.Dependencias
                    .Where(d => d.TareaPredecesoraId == tareaActual.Id)
                    .Select(d => d.TareaSucesora)
                    .ToListAsync();

                foreach (var sucesora in sucesoras)
                {
                    sucesora.FechaInicio = tareaActual.FechaFin.AddDays(1);
                    _context.Tareas.Update(sucesora);
                    await _context.SaveChangesAsync();

                    tareasRecalculadas.Add(sucesora);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ModificarFechaTarea(int tareaId, DateTime fechaFin)
        {
            var tarea = await _context.Tareas.FindAsync(tareaId);
            if (tarea == null)
            {
                ViewBag.Message = "Tarea no encontrada.";
                ViewBag.Success = false;
                var tareas = await _context.Tareas.ToListAsync();
                ViewBag.Tareas = tareas;
                return View();
            }

            tarea.FechaFin = fechaFin;
            _context.Tareas.Update(tarea);
            await _context.SaveChangesAsync();

            // Recalcular las fechas de las tareas dependientes
            await RecalcularCronograma(tarea);

            ViewBag.Message = "Fecha de tarea modificada exitosamente.";
            ViewBag.Success = true;

            var tareasActualizadas = await _context.Tareas.ToListAsync();
            ViewBag.Tareas = tareasActualizadas;

            return View();
        }
    }
}