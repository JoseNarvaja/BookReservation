using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;

namespace BookReservationAPI.Services.Interfaces
{
    public interface IReservationsService
    {
        Task<IEnumerable<Reservation>> GetAllAsync(string jwt, int pageSize = 5, int pageNumber = 1);
        Task<Reservation> ReserveAsync(ReservationCreateDto reservationCreate, string jwt);
        Task<Reservation> PickUpAsync(int id);
        Task<Reservation> ReturnReservationAsync(int id);
        Task<Reservation> GetReservationAsync(int id);
    }
}
