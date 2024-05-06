using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace BookReservationAPI.Services
{
    public class Service<TEntity, TID> : IService<TEntity, TID> where TEntity : class
    {
        private readonly IRepository<TEntity> _repository;

        public Service(IRepository<TEntity> repository)
        {
            _repository = repository;
        }
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(int pageSize, int pageNumber)
        {
            return await _repository.GetAllAsync(pageNumber: pageNumber, pageSize: pageSize);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _repository.GetAsync(filter);
        }
    }
}
