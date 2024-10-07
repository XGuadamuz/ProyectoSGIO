using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.Controllers
{
    [ResponseCache(Duration = 0, NoStore = true)]
    public class ProveedoresController : Controller
    {
        private readonly AppDBContext _dbContext;

        public ProveedoresController(AppDBContext appDBContext)
        {
            _dbContext = appDBContext;
        }

        // GET: Proveedores/VisualizarProveedores
        [HttpGet]
        public IActionResult VisualizarProveedores()
        {
            var proveedores = _dbContext.Proveedores.ToList(); // Obtener todos los proveedores
            return View(proveedores); // Pasar proveedores a la vista
        }

        // GET: Proveedores/RegistroProveedor
        [HttpGet]
        public IActionResult RegistroProveedor()
        {
            return View();
        }

        // POST: Proveedores/RegistroProveedor
        [HttpPost]
        public IActionResult RegistroProveedor(Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                proveedor.Estado = true; // Activo por defecto
                _dbContext.Proveedores.Add(proveedor);
                _dbContext.SaveChanges();
                return RedirectToAction("VisualizarProveedores"); // Redirigir a la lista de proveedores
            }
            return View(proveedor); // Si hay errores, volver a la vista de registro
        }
    }
}
