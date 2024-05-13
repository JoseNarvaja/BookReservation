using BookReservationAPI.Models;

namespace BookReservationAPI.Services.Interfaces
{
    public interface IBooksService : IService<Book,int>
    {
        Task<Book> GetBookByISBNAsync(string ISBN);
        Task UpdateAsync(Book category, string ISBN);
        Task DeleteAsync(string ISBN);
    }
}
