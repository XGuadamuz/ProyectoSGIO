using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using ProyectoSGIOCore.ViewModels;
using System.Security.Claims;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EmpleadoController : Controller
    {
        private readonly AppDBContext _dbContext;
        public EmpleadoController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> VisualizarEmpleados()
        {
            // Obtener los empleados de la base de datos
            var empleados = await _dbContext.Empleados
                                           .ToListAsync();
            return View(empleados);
        }

        [HttpGet]
        public IActionResult CrearEmpleado()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearEmpleado(Empleado modelo)
        {
            // Verifica si el correo ya está en uso
            var empleadoExistente = await _dbContext.Empleados
                                                   .FirstOrDefaultAsync(e => e.Correo == modelo.Correo);

            if (empleadoExistente != null)
            {
                ViewData["Mensaje"] = "El correo electrónico ya está en uso.";
                return View(modelo);
            }

            // Crea el empleado
            Empleado empleado = new Empleado()
            {
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Correo = modelo.Correo,
            };

            // Guarda el empleado en la base de datos
            await _dbContext.Empleados.AddAsync(empleado);
            await _dbContext.SaveChangesAsync();

            // Redirige si se creó correctamente
            if (empleado.IdEmpleado != 0)
            {
                TempData["MensajeExito"] = "Empleado creado correctamente";
                return RedirectToAction("CrearEmpleado", "Empleado");
            }
            else
            {
                ViewData["Mensaje"] = "No se pudo crear el empleado";
            }
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> EditarEmpleado(int id)
        {
            var empleado = await _dbContext.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        [HttpPost]
        public async Task<IActionResult> EditarEmpleado(Empleado entidad)
        {
            var empleado = await _dbContext.Empleados.FindAsync(entidad.IdEmpleado);
            
            // Verificar si el empleado existe 
            if (empleado == null)
            {
                return NotFound();
            }

            empleado.Nombre = entidad.Nombre;
            empleado.Apellido = entidad.Apellido;
            empleado.Correo = entidad.Correo;
            await _dbContext.SaveChangesAsync();

            TempData["MensajeExito"] = "empleado editado exitosamente.";
            return RedirectToAction("VisualizarEmpleados");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
