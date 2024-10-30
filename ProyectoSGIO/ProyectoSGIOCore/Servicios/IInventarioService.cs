using ProyectoSGIOCore.Models;

namespace ProyectoSGIOCore.Servicios
{
    public interface IInventarioService
    {
        Task<(bool success, string message)> ActualizarInventarioAsync(int id, int cantidad);
        Task<bool> VerificarNivelBajoInventarioAsync(Inventario inventario);
        Task EnviarAlertaInventarioBajoAsync(Inventario inventario);
        Task<Inventario> ConsultarInventarioAsync(int id, string usuarioId);
    }
}
