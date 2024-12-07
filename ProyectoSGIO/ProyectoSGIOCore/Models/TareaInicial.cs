using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoSGIOCore.Models
{
    public class TareaInicial
    {
        public int Id { get; set; }
        public int FaseInicialId { get; set; }

        [ForeignKey("FaseInicialId")]
        public FaseInicial FaseInicial { get; set; }

        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

}
