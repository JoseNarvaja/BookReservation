using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repositories.Interfaces;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;

namespace BookReservationAPI.Utility.ReservationValidation
{
    public class ReservationValidator : IReservationValidator
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICopiesRepository _copiesRepository;
        private const int MaxReservationDays = 7;

        public ReservationValidator(IBookRepository bookRepository, ICopiesRepository copiesRepository)
        {
            _bookRepository = bookRepository;
            _copiesRepository = copiesRepository;
        }

        public async Task Validate(ReservationCreateDto reservationCreateDto)
        {
            await ValidateBook(reservationCreateDto);
            await ValidateDate(reservationCreateDto);
        }

        private async Task ValidateBook(ReservationCreateDto reservation)
        {
            Book bookFromDb = await _bookRepository.GetAsync(book => book.ISBN == reservation.ISBN);

            if (bookFromDb == null)
            {
                throw new KeyNotFoundException("The book doesn't exist");
            }

            if(_copiesRepository.IsCopyAvailable(bookFromDb.Id))
            {
                throw new KeyNotFoundException("No available copy was found");
            }
        }

        private async Task ValidateDate(ReservationCreateDto reservation)
        {
            DateTime reservationDate = reservation.ReservationDate;
            DateTime reservationEnd = reservation.ReservationEnd;
            DateTime currentDate = DateTime.Now;

            if (reservationDate < currentDate || reservationEnd < currentDate)
            {
                throw new ArgumentException("The reservation date cannot be earlier than the current moment");
            }

            if (reservationEnd < reservationDate)
            {
                throw new ArgumentException("The reservation end date cannot be earlier than the reservation date.");
            }

            int daysDifference = (reservationEnd - reservationDate).Days;
            if (daysDifference > MaxReservationDays)
            {
                throw new ArgumentException("The reservation cannot exceed " + MaxReservationDays.ToString() + " days.");
            }
        }
    }
}
