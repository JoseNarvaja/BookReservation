using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;
using BookReservationAPI.Utility.ReservationValidation.Validators;

namespace BookReservationAPI.Utility.ReservationValidation
{
    public class ReservationValidator : IReservationValidator
    {
        private List<IReservationValidator> _validators;

        public ReservationValidator()
        {
            _validators= new List<IReservationValidator>();
            _validators.Add(new BookValidator());
            _validators.Add(new DateValidator());
        }

        public async Task<ReservationValidatorResult> Validate(IUnitOfWork unitOfWork, ReservationCreateDto reservationCreateDto)
        {
            ReservationValidatorResult result = new();
            result.success = true;

            foreach (var validator in _validators)
            {
                ReservationValidatorResult validatorResult = await validator.Validate(unitOfWork, reservationCreateDto);
                if (! validatorResult.success)
                {
                    result.success = false;
                    result.Errors.AddRange(validatorResult.Errors);
                }
            }
            return result;
        }
    }
}
