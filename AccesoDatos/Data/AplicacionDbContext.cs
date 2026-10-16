using AccesoDatos.Models;
using Microsoft.EntityFrameworkCore;

namespace AccesoDatos.Data
{
    public class AplicacionDbContext : DbContext
    {
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gym.db")}");
        }
    }
}