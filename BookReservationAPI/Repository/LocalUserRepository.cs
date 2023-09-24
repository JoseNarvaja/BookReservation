using BookReservationAPI.Data;
using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;

namespace BookReservationAPI.Repository
{
    public class LocalUserRepository : Repository<LocalUser>, ILocalUserRepository
    {
        private readonly AppDbContext _context;
        public LocalUserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(LocalUser user)
        {
            _context.LocalUsers.Update(user);
        }
    }
}
