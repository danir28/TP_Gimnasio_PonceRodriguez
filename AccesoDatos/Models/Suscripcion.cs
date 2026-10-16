namespace AccesoDatos.Models
{
    public class Suscripcion
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int PlanId { get; set; }
        public int SocioId { get; set; }
        public Plan Plan { get; set; }
        public Socio Socio { get; set; }
        public List<Pago> Pagos { get; set; }
    }
}