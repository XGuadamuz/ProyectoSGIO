namespace ProyectoSGIOCore.Models
{
    public class EvaluacionDesempeño
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public string Metas { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public string Criterios { get; set; }
        public bool Pendiente { get; set; }
    }
}
