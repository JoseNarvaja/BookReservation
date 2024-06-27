using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;

namespace BookReservationAPI.Services
{
    public class CategoriesService : Service<Category, int>, ICategoriesService
    {
        private readonly ICategoryRepository _repository;
        public CategoriesService(ICategoryRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            validatePositiveID(id);

            Category categoryFromDb = await base.GetAsync(c => c.Id == id);

            if (categoryFromDb == null)
            {
                throw new KeyNotFoundException(message: "Unable to find the specified category");
            }
            return categoryFromDb;
        }

        public override async Task<Category> CreateAsync(Category entity)
        {
            if(entity.Id != 0)
            {
                throw new ArgumentException("The id has to be 0");
            }

            ValidateEntity(entity);

            return await base.CreateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            Category dbCategory = await this.GetByIdAsync(id);
            await base.DeleteAsync(dbCategory);
        }

        public async Task UpdateAsync(Category category, int id)
        {ValidateEntity(category);

            if(category.Id != id)
            {
                throw new ArgumentException("The ID in the model does not match the parameter ID.");
            }

            Category dbCategory = await _repository.GetAsync(c => c.Id == id);

            if(dbCategory is null)
            {
                throw new KeyNotFoundException("The ID doesnt match an existing category");
            }

            _repository.Update(category);
            await _repository.SaveAsync();
        }

        private void validatePositiveID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid ID");
            }
        }
    }
}
