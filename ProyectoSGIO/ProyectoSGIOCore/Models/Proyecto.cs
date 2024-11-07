namespace ProyectoSGIOCore.Models
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Relación con Tareas
        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}