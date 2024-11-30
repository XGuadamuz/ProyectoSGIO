using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoSGIOCore.Controllers
{
    public class ArchivosController : Controller
    {
        private readonly AppDBContext _dbContext;

        public ArchivosController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult SubirArchivo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubirArchivo(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewData["Mensaje"] = "No se seleccionó ningún archivo.";
                return View();
            }

            // Validar el tamaño máximo
            if (archivo.Length > 300 * 1024 * 1024)
            {
                ViewData["Mensaje"] = "El archivo excede el tamaño máximo permitido (300 MB).";
                return View();
            }

            // Validar el formato del archivo
            string[] formatosPermitidos = { ".pdf", ".docx", ".xlsx", ".jpg", ".png" };
            string extension = Path.GetExtension(archivo.FileName).ToLower();

            if (!Array.Exists(formatosPermitidos, f => f == extension))
            {
                ViewData["Mensaje"] = $"El formato del archivo no está permitido: {extension}";
                return View();
            }

            try
            {
                // Guardar el archivo en el sistema de archivos
                string rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosSubidos");
                if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                string rutaArchivo = Path.Combine(rutaCarpeta, archivo.FileName);
                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                // Guardar la URL en la base de datos
                var nuevoArchivo = new Archivo
                {
                    Nombre = archivo.FileName,
                    Url = rutaArchivo,
                    FechaSubida = DateTime.Now,
                    UsuarioId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value)
                };

                _dbContext.Archivos.Add(nuevoArchivo);
                await _dbContext.SaveChangesAsync();

                ViewData["Mensaje"] = "Archivo subido exitosamente.";
            }
            catch(Exception)
            {
                ViewData["Mensaje"] = "Hubo un problema de conexión y no se pudo subir el archivo. Por favor, inténtelo de nuevo.";
            }
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerArchivosRecientes()
        {
            var archivosRecientes = await _dbContext.Archivos
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaSubida)
                .Take(10) // Mostrar los 10 archivos más recientes
                .ToListAsync();
            return View(archivosRecientes); 
        }

        [HttpPost]
        public async Task<IActionResult> EliminarArchivo(int id)
        {
            var archivo = await _dbContext.Archivos.FindAsync(id);
            if (archivo == null)
            {
                ViewData["Mensaje"] = "Archivo no encontrado.";
                return RedirectToAction(nameof(VerArchivosRecientes));
            } 
            
            // Eliminar el archivo del sistema de archivos
            string rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosSubidos", archivo.Nombre);
            if (System.IO.File.Exists(rutaArchivo)) 
            { 
                System.IO.File.Delete(rutaArchivo);
            } 
            
            // Eliminar la entrada de la base de datos
            _dbContext.Archivos.Remove(archivo);
            await _dbContext.SaveChangesAsync();
            ViewData["Mensaje"] = "Archivo eliminado exitosamente.";
            return RedirectToAction(nameof(VerArchivosRecientes));
        }

        [HttpGet] public async Task<IActionResult> BuscarArchivos(string criterio)
        {
            var archivos = await _dbContext.Archivos
                .Include(a => a.Usuario)
                .Where(a => a.Nombre.Contains(criterio) || 
                a.Usuario.Nombre.Contains(criterio) || 
                a.FechaSubida.ToString().Contains(criterio) || 
                a.IdArchivo.ToString().Contains(criterio))
                .ToListAsync();
            return View(archivos);
        }

        [HttpGet] public async Task<IActionResult> DescargarArchivo(int id)
        {
            var archivo = await _dbContext.Archivos.FindAsync(id);
            if (archivo == null)
            {
                ViewData["Mensaje"] = "Archivo no encontrado.";
                return RedirectToAction(nameof(VerArchivosRecientes));
            } 
            
            string rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosSubidos", archivo.Nombre);
            if (!System.IO.File.Exists(rutaArchivo))
            {
                ViewData["Mensaje"] = "El archivo no existe en el sistema de archivos.";
                return RedirectToAction(nameof(VerArchivosRecientes));
            }
            
            var memory = new MemoryStream();
            using (var stream = new FileStream(rutaArchivo, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            
            memory.Position = 0; return File(memory, GetContentType(rutaArchivo), archivo.Nombre);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }
    }
}
