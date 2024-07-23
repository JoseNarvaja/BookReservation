using BookReservationAPI.Data;
using BookReservationAPI.Models;
using BookReservationAPI.Repositories.Interfaces;
using BookReservationAPI.Repository;

namespace BookReservationAPI.Repositories
{
    public class CopyRepository : Repository<Copy>, ICopyRepository
    {
        private readonly AppDbContext _context;
        public CopyRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Copy copy)
        {
            _context.Copies.Update(copy);
        }
    }
}
