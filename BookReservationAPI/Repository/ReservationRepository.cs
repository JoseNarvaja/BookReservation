using BookReservationAPI.Data;
using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;

namespace BookReservationAPI.Repository
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        private readonly AppDbContext _context;
        public ReservationRepository(AppDbContext context) : base(context)
        {
            _context= context;
        }

        public async Task NotifyPickup(Reservation reservation)
        {
            if(reservation.PickupDate != null)
            {
                throw new Exception("The book was already picked up.");
            }
            reservation.PickupDate = DateTime.Now;
            await _context.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task NotifyReturn(Reservation reservation)
        {
            if(reservation.ReturnDate != null)
            {
                throw new Exception("The book was already returned.");
            }
            reservation.ReturnDate = DateTime.Now;
            await _context.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
