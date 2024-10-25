using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ProyectoSGIOCore.Controllers
{
    public class FacturasController : Controller
    {
        private readonly AppDBContext _dbContext;

        public FacturasController(AppDBContext context)
        {
            _dbContext = context;
        }

        // GET: Facturas/RegistroFactura
        [HttpGet]
        public IActionResult RegistroFactura()
        {
            ViewBag.Proveedores = new SelectList(_dbContext.Proveedores, "IdProveedor", "Nombre");
            return View();
        }

        // POST: Facturas/RegistroFactura
        [HttpPost]
        public IActionResult RegistrarFactura(FacturaProveedor factura)
        {
            if (ModelState.IsValid)
            {
                var proveedor = _dbContext.Proveedores.Find(factura.IdProveedor);
                if (proveedor == null)
                {
                    ModelState.AddModelError("", "El proveedor seleccionado no existe.");
                    return View(factura);
                }

                factura.Proveedor = proveedor; // Asignar la relación
                _dbContext.Facturas.Add(factura);
                _dbContext.SaveChanges();
                return RedirectToAction("VisualizarFacturas");
            }

            return View(factura);
        }

        // GET: Facturas/VisualizarFacturas
        [HttpGet]
        public IActionResult VisualizarFacturas()
        {
            var facturas = _dbContext.Facturas
                .Include(f => f.Proveedor) // Cargar la información del proveedor relacionado
                .ToList();

            return View(facturas);
        }
    }
}
