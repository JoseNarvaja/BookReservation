using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Models.Pagination;

namespace BookReservationAPI.Services.Interfaces
{
    public interface IReservationsService
    {
        Task<(IEnumerable<Reservation>, int)> GetAllWithCountAsync(string jwt, PaginationParams pagination);
        Task<Reservation> ReserveAsync(ReservationCreateDto reservationCreate, string jwt);
        Task<Reservation> PickUpAsync(int id);
        Task<Reservation> ReturnReservationAsync(int id);
        Task<Reservation> GetReservationAsync(int id);
    }
}
