using BookReservationAPI.Models;

namespace BookReservationAPI.Repository.Interfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task NotifyReturn(Reservation reservation);
        Task NotifyPickup(Reservation reservation);
    }
}
