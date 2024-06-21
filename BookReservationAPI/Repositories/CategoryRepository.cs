using BookReservationAPI.Data;
using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;

namespace BookReservationAPI.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _dbContext;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public void Update(Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}
