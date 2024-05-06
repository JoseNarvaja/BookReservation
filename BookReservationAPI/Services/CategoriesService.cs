using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Net.Security;

namespace BookReservationAPI.Services
{
    public class CategoriesService : Service<Category, int>, ICategoriesService
    {
        private readonly ICategoryRepository _repository;
        public CategoriesService(ICategoryRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            validateID(id);

            Category categoryFromDb = await base.GetAsync(c => c.Id == id);

            if (categoryFromDb == null)
            {
                throw new KeyNotFoundException(message: "No category was found");
            }
            return categoryFromDb;
        }

        public override async Task<Category> CreateAsync(Category entity)
        {
            validateEntity(entity);

            return await base.CreateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            Category dbCategory = await this.GetCategoryAsync(id);
            _repository.Remove(dbCategory);
            await _repository.SaveAsync();
        }

        public async Task UpdateAsync(Category category, int id)
        {
            validateEntity(category);

            if(category.Id != id)
            {
                throw new ArgumentException("The ID in the model does not match the parameter ID.");
            }

            _repository.Update(category);
            await _repository.SaveAsync();
        }

        private void validateEntity(Category entity)
        {
            if(entity == null)
            {
                throw new ArgumentException("The category attributes can't be null");
            }

            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(entity, validationContext, validationResults, validateAllProperties: true))
            {
                string errorMessage = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }
        }

        private void validateID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid ID");
            }
        }
    }
}
