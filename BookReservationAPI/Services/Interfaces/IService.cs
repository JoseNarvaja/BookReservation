using BookReservationAPI.Models.Pagination;
using System.Linq.Expressions;

namespace BookReservationAPI.Services.Interfaces
{
    public interface IService<TEntity, TID> where TEntity : class
    {
        Task<(IEnumerable<TEntity>, int)> GetAllWithTotalCountAsync(PaginationParams pagination,Expression<Func<TEntity, bool>>? filter = null, string? includeProperties = null);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> CreateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
