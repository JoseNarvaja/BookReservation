using BookReservationAPI.Models.Pagination;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
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

        public async Task DeleteAsync(TEntity entity)
        {
            _repository.Remove(entity);
            await _repository.SaveAsync();
        }

        public async Task<(IEnumerable<TEntity>, int)> GetAllWithTotalCountAsync(PaginationParams pagination, Expression<Func<TEntity, bool>>? filter = null, string? includeProperties = null)
        {
            IEnumerable<TEntity> entities = await _repository.GetAllAsync(pagination, filter, includeProperties: includeProperties);
            int totalCount = await _repository.GetTotalCountAsync();
            return (entities, totalCount);
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _repository.GetAsync(filter);
        }

        protected virtual void ValidateEntity(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("An entity attribute is missing");
            }

            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(entity, validationContext, validationResults, validateAllProperties: true))
            {
                string errorMessage = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
        }
    }
}
