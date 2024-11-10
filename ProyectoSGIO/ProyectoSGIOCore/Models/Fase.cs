namespace ProyectoSGIOCore.Models
{
    public class Fase
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; }
        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}
