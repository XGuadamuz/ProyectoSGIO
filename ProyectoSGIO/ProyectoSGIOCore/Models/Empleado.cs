namespace ProyectoSGIOCore.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public decimal HorasExtraAcumuladas { get; set; }
        public decimal UmbralHorasExtra { get; set; }

        public double TiempoLibre { get; set; }
        public List<EvaluacionDesempeño> Evaluaciones { get; set; } = new List<EvaluacionDesempeño>();

        public bool SuperaUmbralHorasExtra()
        {
            return HorasExtraAcumuladas > UmbralHorasExtra;
        }
    }
}
