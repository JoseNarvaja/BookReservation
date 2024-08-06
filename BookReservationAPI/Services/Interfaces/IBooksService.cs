using BookReservationAPI.Models;
using BookReservationAPI.Models.Pagination;
using System.Linq.Expressions;

namespace BookReservationAPI.Services.Interfaces
{
    public interface IBooksService : IService<Book,int>
    {
        Task<(IEnumerable<Book>, int)> GetAllWithTotalCountAsync(BooksParams bookParams);
        Task<Book> GetBookByISBNAsync(string ISBN);
        Task UpdateAsync(Book category, string ISBN);
        Task DeleteAsync(string ISBN);
    }
}
