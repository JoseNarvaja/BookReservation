using BookReservationAPI.Data;
using BookReservationAPI.Models;
using BookReservationAPI.Repositories.Interfaces;
using BookReservationAPI.Repository;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace BookReservationAPI.Repositories
{
    public class CopiesRepository : Repository<Copy>, ICopiesRepository
    {
        private readonly AppDbContext _context;
        public CopiesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetAvailableCopiesCountByISBN(string isbn)
        {
            return await _context.Copies
                .CountAsync(c => c.Book.ISBN == isbn && c.IsAvailable);
        }

        public bool IsCopyAvailable(int id)
        {
            return _context.Copies.Any(c => c.IsAvailable && !c.IsDeleted && c.BookId == id);
        }

        public async Task MarkAsAvailable(int id)
        {
            var copy = await _context.Copies.FindAsync(id);
            copy.IsAvailable = true;

            _context.Update(copy);
            _context.SaveChanges();
        }

        public async Task MarkAsUnavailable(int id)
        {
            var copy = await _context.Copies.FindAsync(id);
            copy.IsAvailable = false;

            _context.Update(copy);
            _context.SaveChanges();
        }

        public void Update(Copy copy)
        {
            _context.Copies.Update(copy);
        }
    }
}
