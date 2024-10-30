using ProyectoSGIOCore.Data;
using ProyectoSGIOCore.Servicios;
using Microsoft.EntityFrameworkCore;
using ProyectoSGIOCore.Models;
using Microsoft.Extensions.Logging;

namespace ProyectoSGIOCore.Servicios
{
    public class InventarioService : IInventarioService
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<InventarioService> _logger;
        private readonly int _maxRetries = 3; // Número máximo de reintentos para enviar alertas
        private readonly int _retryDelaySeconds = 30; // Tiempo entre reintentos

        public InventarioService(
            AppDBContext dbContext,
            ILogger<InventarioService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Escenario 1 y 4: Actualización del inventario con manejo de errores
        public async Task<(bool success, string message)> ActualizarInventarioAsync(int id, int cantidad)
        {
            try
            {
                var inventario = await _dbContext.Inventarios
                    .Include(i => i.Proveedor)
                    .FirstOrDefaultAsync(i => i.ID == id);

                if (inventario == null)
                    return (false, "Material no encontrado en el inventario.");

                inventario.Cantidad = cantidad;
                inventario.UltimaActualizacion = DateTime.Now;
                inventario.PrecioTotal = inventario.Cantidad * inventario.PrecioUnidad;

                await _dbContext.SaveChangesAsync();

                // Escenario 2: Verificar nivel bajo y enviar alerta si es necesario
                if (await VerificarNivelBajoInventarioAsync(inventario))
                {
                    _ = EnviarAlertaInventarioBajoAsync(inventario); // Ejecutar de manera asíncrona
                }

                return (true, "Inventario actualizado correctamente.");
            }
            catch (DbUpdateException)
            {
                return (false, "Error de conexión con la base de datos. Por favor, intente más tarde.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar el inventario");
                return (false, "Error inesperado al actualizar el inventario.");
            }
        }

        // Escenario 2: Verificación de nivel bajo
        public async Task<bool> VerificarNivelBajoInventarioAsync(Inventario inventario)
        {
            return await Task.FromResult(inventario.Cantidad < inventario.CantidadMinima);
        }

        // Escenario 2 y 5: Envío de alertas con reintentos
        public async Task EnviarAlertaInventarioBajoAsync(Inventario inventario)
        {
            int intentos = 0;
            bool alertaEnviada = false;

            while (!alertaEnviada && intentos < _maxRetries)
            {
                try
                {
                    
                    _logger.LogWarning(
                        $"ALERTA: Nivel bajo de inventario\n" +
                        $"Material: {inventario.Nombre}\n" +
                        $"Cantidad actual: {inventario.Cantidad}\n" +
                        $"Cantidad mínima: {inventario.CantidadMinima}\n" +
                        $"Proveedor: {inventario.Proveedor?.Nombre ?? "No especificado"}"
                    );

                    alertaEnviada = true;
                }
                catch (Exception ex)
                {
                    intentos++;
                    _logger.LogError(ex, $"Error al enviar alerta. Intento {intentos} de {_maxRetries}");

                    if (intentos < _maxRetries)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_retryDelaySeconds));
                    }
                }
            }

            if (!alertaEnviada)
            {
                _logger.LogError($"No se pudo enviar la alerta después de {_maxRetries} intentos");
            }
        }

        // Escenario 3: Consulta de inventario
        public async Task<Inventario> ConsultarInventarioAsync(int id, string usuarioId)
        {
            try
            {
                // Aquí podrías agregar verificación de permisos del usuario
                var inventario = await _dbContext.Inventarios
                    .Include(i => i.Proveedor)
                    .FirstOrDefaultAsync(i => i.ID == id);

                if (inventario == null)
                {
                    _logger.LogWarning($"Intento de consultar inventario inexistente. ID: {id}");
                    return null;
                }

                return inventario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar el inventario");
                throw;
            }
        }
    }
}
