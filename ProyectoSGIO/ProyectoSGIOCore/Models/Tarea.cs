using System.ComponentModel.DataAnnotations;

namespace ProyectoSGIOCore.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Completada { get; set; } = false;

        // Relación con el Proyecto
        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; }
    }
}