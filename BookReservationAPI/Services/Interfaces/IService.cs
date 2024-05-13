using System.Linq.Expressions;

namespace BookReservationAPI.Services.Interfaces
{
    public interface IService<TEntity, TID> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(int pageSize = 5, int pageNumber = 1);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> CreateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
