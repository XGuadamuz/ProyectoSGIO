namespace ProyectoSGIOCore.Models
{
    public class HorasExtra
    {
        public int IdHorasExtra { get; set; }
        public int IdEmpleado { get; set; } // Relación con empleado
        public DateTime Fecha { get; set; }
        public double CantidadHoras { get; set; }
        public bool Aprobada { get; set; } = false;

        // Relación con el modelo Empleado
        public virtual Empleado Empleado { get; set; }
    }
}
