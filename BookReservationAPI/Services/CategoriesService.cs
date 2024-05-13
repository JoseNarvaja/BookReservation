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
            ValidateEntity(entity);

            return await base.CreateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            Category dbCategory = await this.GetCategoryAsync(id);
            await base.DeleteAsync(dbCategory);
        }

        public async Task UpdateAsync(Category category, int id)
        {
            ValidateEntity(category);

            if(category.Id != id)
            {
                throw new ArgumentException("The ID in the model does not match the parameter ID.");
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
