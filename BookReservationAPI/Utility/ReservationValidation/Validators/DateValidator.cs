using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;

namespace BookReservationAPI.Utility.ReservationValidation.Validators
{
    public class DateValidator : IReservationValidator
    {
        private const int MaxReservationDays = 7;
        public async Task<ReservationValidatorResult> Validate(IUnitOfWork unitOfWork, ReservationCreateDto reservationCreateDto)
        {
            ReservationValidatorResult result = new();
            result.success = true;

            DateTime reservationDate = reservationCreateDto.ReservationDate;
            DateTime reservationEnd = reservationCreateDto.ReservationEnd;
            DateTime currentDate = DateTime.Now;

            if(reservationDate < currentDate || reservationEnd < currentDate)
            {
                result.success = false;
                result.Errors.Add("The date cannot be earlier than the current moment");
            }

            if(reservationEnd < reservationDate)
            {
                result.success = false;
                result.Errors.Add("The reservation end date cannot be earlier than the reservation date.");
            }

            int daysDifference = (reservationEnd - reservationDate).Days;
            if(daysDifference > MaxReservationDays)
            {
                result.success = false;
                result.Errors.Add("The reservation cannot exceed " + MaxReservationDays.ToString() + " days.");
            }

            return result;
        }
    }
}
