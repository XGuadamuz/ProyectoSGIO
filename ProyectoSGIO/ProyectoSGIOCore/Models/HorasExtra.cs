namespace ProyectoSGIOCore.Models
{
    public class HorasExtra
    {
        public int IdHorasExtra { get; set; }
        public int IdEmpleado { get; set; }
        public Empleado Empleado { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadHoras { get; set; }
        public EstadoHorasExtra Estado { get; set; }
        public TipoCompensacion TipoCompensacion { get; set; }
        public string Descripcion { get; set; }
        public int? IdSupervisor { get; set; }
        public Usuario? Supervisor { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }

    // Enumeraciones para manejar estados y tipos de compensación
    public enum EstadoHorasExtra
    {
        Pendiente,
        Aprobado,
        Rechazado
    }

    public enum TipoCompensacion
    {
        PagoAdicional,
        TiempoLibre
    }
}
