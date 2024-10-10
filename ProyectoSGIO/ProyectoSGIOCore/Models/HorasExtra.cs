namespace ProyectoSGIOCore.Models
{
    public class HoraExtra
    {
        public int IdHoraExtra { get; set; }
        public int IdEmpleado { get; set; }
        public Empleado Empleado { get; set; }
        public DateTime Fecha { get; set; }
        public double CantidadHoras { get; set; }
        public bool Aprobada { get; set; } // Indica si fue aprobada
    }
}
