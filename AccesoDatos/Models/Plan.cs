namespace AccesoDatos.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int ProfesorId { get; set; }
        public Profesor Profesor { get; set; }
        public List<Suscripcion> Suscripciones { get; set; }
    }
}