using Microsoft.AspNetCore.Mvc;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoSGIOCore.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministrativoController : Controller
    {
        private readonly AppDBContext _dbContext;

        public AdministrativoController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {

            var roles = _dbContext.Roles.ToList();
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(CrearUsuarioVM modelo)
        {
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View(modelo);
            }

            var rolDefault = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Nombre == modelo.RolSeleccionado);

            Usuario usuario = new Usuario()
            {
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Correo = modelo.Correo,
                Clave = modelo.Clave,
                IdRol = rolDefault?.IdRol ?? 0
            };

            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            if (usuario.IdUsuario != 0) return RedirectToAction("CrearUsuario", "Administrativo");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View(modelo);
        }
    }
}