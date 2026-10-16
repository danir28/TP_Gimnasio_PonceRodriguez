namespace AccesoDatos.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public int SuscripcionId { get; set; }
        public Suscripcion Suscripciones { get; set; }
    }
}