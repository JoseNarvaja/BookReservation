using BookReservationAPI.Data;
using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookReservationAPI.Repository
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public BookRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DecreaseCount(string ISBN, int count)
        {
            Book book = await _dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == ISBN);
            book.Stock -= count;
            _dbContext.Books.Update(book);
            return;
        }

        public void Update(Book book)
        {
            _dbContext.Books.Update(book);
        }
    }
}
