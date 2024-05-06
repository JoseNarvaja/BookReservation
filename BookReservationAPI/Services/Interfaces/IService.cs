using System.Linq.Expressions;

namespace BookReservationAPI.Services.Interfaces
{
    public interface IService<TEntity, TID> where TEntity : class
    {
        public Task<IEnumerable<TEntity>> GetAllAsync(int pageSize = 5, int pageNumber = 1);
        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter);
        public Task<TEntity> CreateAsync(TEntity entity);
    }
}
