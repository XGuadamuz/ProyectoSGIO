using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly AppDBContext _dbContext;

        public ProyectoController(AppDBContext context)
        {
            _dbContext = context;
        }

        // Método para crear un nuevo proyecto
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                proyecto.FechaCreacion = DateTime.Now;
                _dbContext.Proyectos.Add(proyecto);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(proyecto);
        }

        // Método para agregar tareas a un proyecto existente
        public async Task<IActionResult> AgregarTareas(int id)
        {
            var proyecto = await _dbContext.Proyectos.Include(p => p.Tareas)
                                                   .FirstOrDefaultAsync(p => p.Id == id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTareas(int id, List<Tarea> tareas)
        {
            var proyecto = await _dbContext.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }

            foreach (var tarea in tareas)
            {
                tarea.ProyectoId = id;
                _dbContext.Tareas.Add(tarea);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Detalles", new { id });
        }

        // Método para ver detalles del proyecto junto con sus tareas
        public async Task<IActionResult> Detalles(int id)
        {
            var proyecto = await _dbContext.Proyectos.Include(p => p.Tareas)
                                                   .FirstOrDefaultAsync(p => p.Id == id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        // Método para ver todos los proyectos
        public async Task<IActionResult> Index()
        {
            var proyectos = await _dbContext.Proyectos.ToListAsync();
            return View(proyectos);
        }
    }
}