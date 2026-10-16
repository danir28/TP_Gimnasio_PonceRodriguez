using AccesoDatos.Data;
using Microsoft.EntityFrameworkCore;

namespace AccesoDatos.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AplicacionDbContext _context;

        public GenericRepository(AplicacionDbContext context)
        {
            _context = context;
        }

        public void agregar(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void modificar(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

        public List<T> ObtenerTodosCon(params string[] propiedadesRelacionadas)
        {
            IQueryable<T> consulta = _context.Set<T>().AsNoTracking();

            foreach (var propiedad in propiedadesRelacionadas)
            {
                consulta = consulta.Include(propiedad);
            }

            return consulta.ToList();
        }

        public T? ObtenerPorId(int id)
        {
            return _context.Set<T>().Find(id);
        }
    }
}