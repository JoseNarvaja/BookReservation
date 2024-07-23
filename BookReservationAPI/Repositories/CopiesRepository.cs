using BookReservationAPI.Data;
using BookReservationAPI.Models;
using BookReservationAPI.Repositories.Interfaces;
using BookReservationAPI.Repository;
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

        public bool IsCopyAvailable(int id)
        {
            return _context.Copies.Any(c => c.IsAvailable && c.BookId == id);
        }

        public async Task MarkAsAvailable(int id)
        {
            var copy = await _context.Copies.FindAsync(id);
            copy.IsAvailable = true;

            _context.Update(copy);
            _context.SaveChanges();
        }

        public void Update(Copy copy)
        {
            _context.Copies.Update(copy);
        }
    }
}
