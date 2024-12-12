namespace ProyectoSGIOCore.Models
{
    public class Dependencia
    {
        public int Id { get; set; }
        public int TareaPredecesoraId { get; set; }
        public Tarea TareaPredecesora { get; set; }
        public int TareaSucesoraId { get; set; }
        public Tarea TareaSucesora { get; set; }
        public string TipoDependencia { get; set; } // FS para Fin a Inicio
    }
}
