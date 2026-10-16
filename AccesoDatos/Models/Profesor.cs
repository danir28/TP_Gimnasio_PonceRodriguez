namespace AccesoDatos.Models
{
    public class Profesor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public List<Plan> Planes { get; set; }
    }
}