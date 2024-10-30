namespace ProyectoSGIOCore.Models
{
    public class Inventario
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public int CantidadMinima { get; set; }
        public decimal PrecioUnidad { get; set; }
        public decimal PrecioTotal { get; set; }
        public bool Stock => Cantidad > 0;
        public string? InformacionAdicional { get; set; }
        public DateTime UltimaActualizacion { get; set; } 
        public virtual Proveedor? Proveedor { get; set; }
    }

}
