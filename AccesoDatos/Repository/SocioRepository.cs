using AccesoDatos.Data;
using AccesoDatos.Models;
using Microsoft.EntityFrameworkCore;

namespace AccesoDatos.Repositories
{
    // Repositorio específico de Socio: hereda lo genérico (agregar, modificar,
    // ObtenerPorId, ObtenerTodosCon) y suma las consultas propias de este modelo.
    public class SocioRepository : GenericRepository<Socio>
    {
        public SocioRepository(AplicacionDbContext context) : base(context)
        {
        }

        // Lista socios. Si 'texto' no está vacío, filtra por DNI o apellido.
        // Si 'soloActivos' es true, oculta a los socios dados de baja.
        public List<Socio> Buscar(string texto, bool soloActivos)
        {
            IQueryable<Socio> consulta = _context.Socios.AsNoTracking();

            if (soloActivos)
            {
                consulta = consulta.Where(s => s.Activo);
            }

            if (!string.IsNullOrWhiteSpace(texto))
            {
                // Like no distingue mayúsculas de minúsculas en SQLite
                consulta = consulta.Where(s =>
                    EF.Functions.Like(s.Dni, $"%{texto}%") ||
                    EF.Functions.Like(s.Apellido, $"%{texto}%"));
            }

            return consulta
                .OrderBy(s => s.Apellido)
                .ThenBy(s => s.Nombre)
                .ToList();
        }

        // Devuelve el socio con ese DNI, o null si no existe.
        // Sirve para no permitir dos socios con el mismo DNI.
        public Socio? ObtenerPorDni(string dni)
        {
            return _context.Socios.FirstOrDefault(s => s.Dni == dni);
        }
    }
}