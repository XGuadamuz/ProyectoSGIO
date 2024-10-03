using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.ViewModels;

namespace ProyectoSGIOCore.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDBContext _dbContext;
        public AccesoController(AppDBContext appDBContext)
        {
            _dbContext = appDBContext;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(UsuarioVM modelo)
        {
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }
            Usuario usuario = new Usuario()
            {
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Correo = modelo.Correo,
                Clave = modelo.Clave
            };

            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            if (usuario.IdUsuario != 0) return RedirectToAction("IniciarSesion", "Acceso");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(IniciarSesionVM modelo)
        {
            Usuario? usuario_encontrado = await _dbContext.Usuarios
                                        .Where(u =>
                                        u.Correo == modelo.Correo &&
                                        u.Clave == modelo.Clave
                                        ).FirstOrDefaultAsync();

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
