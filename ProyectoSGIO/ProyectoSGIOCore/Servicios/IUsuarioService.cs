using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.Servicios
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> ObtenerTodosUsuarios();
        Task<Usuario> ObtenerUsuarioPorId(int id);
        Task<bool> CrearUsuario(Usuario usuario);
        Task<bool> ActualizarUsuario(Usuario usuario);
        Task<bool> DesactivarUsuario(int id);
        Task<bool> EliminarUsuario(int id);
        Task<IEnumerable<Rol>> ObtenerRoles();
    }
}
