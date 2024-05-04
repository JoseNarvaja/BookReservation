using System.Linq.Expressions;

namespace BookReservationAPI.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 5, int pageNumber = 1);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool asTracked = false, string? includeProperties = null);
        Task AddAsync(T entity);
        void Remove(T entity);
        void RemoveRange (IEnumerable<T> entities);
    }
}
