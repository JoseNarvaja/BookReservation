using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;

namespace BookReservationAPI.Utility.ReservationValidation.Validators
{
    public class BookValidator : IReservationValidator
    {
        public async Task<ReservationValidatorResult> Validate(IUnitOfWork unitOfWork, ReservationCreateDto reservationCreateDto)
        {
            ReservationValidatorResult result = new();
            result.success = true;

            Book bookFromDb = await unitOfWork.Books.GetAsync(book => book.ISBN == reservationCreateDto.ISBN);
            if(bookFromDb == null)
            {
                result.success= false;
                result.Errors.Add("The book doesn't exist");
            }
            if(bookFromDb.Stock <= 0)
            {
                result.success= false;
                result.Errors.Add("There is no stock left for this book");
            }

            return result;
        }
    }
}
