namespace AccesoDatos.Models
{
    public class Socio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni {get; set; }
        public string Telefono { get; set; }
        public DateTime FechaAlta { get; set; }
        public bool Activo { get; set; }
        public List<Suscripcion> Suscripciones { get; set; }
    }
}