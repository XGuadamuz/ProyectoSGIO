﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

        [HttpGet]
        public IActionResult DescargarFacturasHTML()
        {
            // Obtener la lista de facturas con su proveedor
            var facturas = _dbContext.Facturas.Include(f => f.Proveedor).ToList();

            // Crear el contenido HTML
            var html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>Reporte de Facturas</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("h1 { text-align: center; color: #333; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: center; }");
            html.AppendLine("th { background-color: #4CAF50; color: white; }");
            html.AppendLine("tr:nth-child(even) { background-color: #f2f2f2; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<h1>Reporte de Facturas</h1>");
            html.AppendLine("<table>");
            html.AppendLine("<thead>");
            html.AppendLine("<tr>");
            html.AppendLine("<th>ID</th>");
            html.AppendLine("<th>Proveedor</th>");
            html.AppendLine("<th>Número Factura</th>");
            html.AppendLine("<th>Fecha Emisión</th>");
            html.AppendLine("<th>Monto Total</th>");
            html.AppendLine("<th>Descripción</th>");
            html.AppendLine("</tr>");
            html.AppendLine("</thead>");
            html.AppendLine("<tbody>");

            // Llenar el cuerpo de la tabla con las facturas
            foreach (var factura in facturas)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{factura.IdFactura}</td>");
                html.AppendLine($"<td>{factura.Proveedor?.Nombre ?? "Sin Proveedor"}</td>");
                html.AppendLine($"<td>{factura.NumeroFactura}</td>");
                html.AppendLine($"<td>{factura.FechaEmision.ToShortDateString()}</td>");
                html.AppendLine($"<td>{factura.MontoTotal:C}</td>");
                html.AppendLine($"<td>{factura.Descripcion}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</tbody>");
            html.AppendLine("</table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            // Convertir el contenido HTML en un array de bytes
            var bytes = Encoding.UTF8.GetBytes(html.ToString());

            // Devolver el archivo HTML para descarga
            return File(bytes, "text/html", "ReporteFacturas.html");
        }
    }
}
