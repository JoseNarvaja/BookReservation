using BookReservationAPI.Models;

namespace BookReservationAPI.Repository.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
