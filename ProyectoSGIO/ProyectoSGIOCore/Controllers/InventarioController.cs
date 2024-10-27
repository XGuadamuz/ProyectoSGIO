using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.Controllers
{
    public class InventarioController : Controller
    {
        private readonly AppDBContext _dbContext;

        public InventarioController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> VisualizarInventario()
        {
            var inventarios = await _dbContext.Inventarios.ToListAsync();
            return View(inventarios);
        }

        [HttpGet]
        public IActionResult CrearInventario() => View();

        [HttpPost]
        public async Task<IActionResult> CrearInventario(Inventario inventario)
        {
            if (!ModelState.IsValid) return View(inventario);

            inventario.PrecioTotal = inventario.Cantidad * inventario.PrecioUnidad;
            _dbContext.Inventarios.Add(inventario);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("VisualizarInventario");
        }

        [HttpGet]
        public async Task<IActionResult> EditarInventario(int id)
        {
            var inventario = await _dbContext.Inventarios.FindAsync(id);
            return inventario == null ? NotFound() : View(inventario);
        }

        [HttpPost]
        public async Task<IActionResult> EditarInventario(Inventario inventario)
        {
            if (!ModelState.IsValid) return View(inventario);

            // Actualiza el Precio Total
            inventario.PrecioTotal = inventario.Cantidad * inventario.PrecioUnidad;

            _dbContext.Inventarios.Update(inventario);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("VisualizarInventario");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarInventario(int id)
        {
            var inventario = await _dbContext.Inventarios.FindAsync(id);
            if (inventario == null) return NotFound();

            _dbContext.Inventarios.Remove(inventario);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("VisualizarInventario");
        }
    }
}