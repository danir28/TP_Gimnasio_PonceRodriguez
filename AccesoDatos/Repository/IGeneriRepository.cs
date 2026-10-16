namespace AccesoDatos.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        void agregar(T entity);
        void modificar(T entity);
        List<T> ObtenerTodosCon(params string[] propiedadesRelacionadas);
        T? ObtenerPorId(int id);
    }
}