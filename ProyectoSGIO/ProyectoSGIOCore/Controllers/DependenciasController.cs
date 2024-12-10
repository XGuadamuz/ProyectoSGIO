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
                    NotificarCambio(dependencia);

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

            var tareas = await _context.Tareas.ToListAsync();
            var fases = await _context.Fases.ToListAsync();
            ViewBag.Tareas = tareas;
            ViewBag.Fases = fases;

            return View();
        }

        private void NotificarCambio(Dependencia dependencia)
        {
            var usuarios = _context.Usuarios.ToList();
            foreach (var usuario in usuarios)
            {
                var subject = "Actualización de Dependencias";
                var message = $"Se ha actualizado la dependencia {dependencia.TipoDependencia} entre la tarea {dependencia.TareaPredecesoraId} y la tarea {dependencia.TareaSucesoraId}.";

                //_utilitarios.EnviarCorreo(usuario.Correo, subject, message);
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
    }
}