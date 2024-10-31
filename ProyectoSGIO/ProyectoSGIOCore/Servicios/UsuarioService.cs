using Microsoft.CodeAnalysis.Scripting;
using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Models;
using ProyectoSGIOCore.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace ProyectoSGIOCore.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(AppDBContext dbContext, ILogger<UsuarioService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosUsuarios()
        {
            return await _dbContext.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();
        }

        public async Task<Usuario> ObtenerUsuarioPorId(int id)
        {
            return await _dbContext.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<bool> CrearUsuario(Usuario usuario)
        {
            try
            {
                // Hashear la contraseña antes de guardarla
                usuario.Clave = BCrypt.Net.BCrypt.HashPassword(usuario.Clave);
                usuario.Activo = true;

                _dbContext.Usuarios.Add(usuario);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return false;
            }
        }

        public async Task<bool> ActualizarUsuario(Usuario usuario)
        {
            try
            {
                var usuarioExistente = await _dbContext.Usuarios.FindAsync(usuario.IdUsuario);
                if (usuarioExistente == null) return false;

                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Apellido = usuario.Apellido;
                usuarioExistente.Correo = usuario.Correo;
                usuarioExistente.IdRol = usuario.IdRol;

                // Solo actualizar la contraseña si se proporciona una nueva
                if (!string.IsNullOrEmpty(usuario.Clave))
                {
                    usuarioExistente.Clave = BCrypt.Net.BCrypt.HashPassword(usuario.Clave);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return false;
            }
        }

        public async Task<bool> DesactivarUsuario(int id)
        {
            try
            {
                var usuario = await _dbContext.Usuarios.FindAsync(id);
                if (usuario == null) return false;

                usuario.Activo = false;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar usuario");
                return false;
            }
        }

        public async Task<bool> EliminarUsuario(int id)
        {
            try
            {
                var usuario = await _dbContext.Usuarios.FindAsync(id);
                if (usuario == null) return false;

                _dbContext.Usuarios.Remove(usuario);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario");
                return false;
            }
        }

        public async Task<IEnumerable<Rol>> ObtenerRoles()
        {
            return await _dbContext.Roles.ToListAsync();
        }
    }
}

