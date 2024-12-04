namespace ProyectoSGIOCore.Models
{
    public class InformeProgreso
    {
        public string Proyecto { get; set; } 
        public string Supervisor { get; set; } 
        public DateTime Fecha { get; set; } 
        public string Estado { get; set; } 
        public List<Tarea> TareasCompletadas { get; set; } 
        public List<Tarea> ProximasTareas { get; set; } 
        public List<string> Incidencias { get; set; }
        public List<Proyecto> Proyectos { get; set; }
    }
}
