namespace WebApplication1.Repositories
{
    public interface IGenericRepository<T>
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        T? Add(T entity);
        bool Update(T entity);
        bool Delete(T entity);
    }
}
