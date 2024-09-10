using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Models.Pagination;
using BookReservationAPI.Repositories.Interfaces;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using BookReservationAPI.Utility;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

namespace BookReservationAPI.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ILocalUserRepository _localUserRepository;
        private readonly ICopiesRepository _copiesRepository; 
        private readonly IReservationValidator _validator;
        private readonly IBookRepository _bookRepository;
        public ReservationsService(IReservationRepository reservationRepository, ILocalUserRepository userRepository,
            IReservationValidator validator, IBookRepository bookRepository, ICopiesRepository copiesRepository)
        {
            _reservationRepository = reservationRepository;
            _localUserRepository = userRepository;
            _bookRepository = bookRepository;
            _copiesRepository = copiesRepository;
            _validator = validator;
        }

        public async Task<(IEnumerable<Reservation>, int)> GetAllWithCountAsync(string jwt, PaginationParams pagination)
        {
            string role = GetClaimFromJwt(jwt, "role");
            int count;

            IEnumerable<Reservation> reservations;
            switch (role)
            {
                case StaticData.RoleAdmin:
                    reservations = await _reservationRepository.GetAllAsync(pagination,includeProperties: "Copy,User,Book");
                    count = await _reservationRepository.GetTotalCountAsync();
                    break;
                case StaticData.RoleCustomer:
                    string username = GetClaimFromJwt(jwt, "unique_name");
                    var user = await _localUserRepository.GetAsync(u => u.UserName == username);
                    reservations = await _reservationRepository.GetAllAsync(pagination, reservation => reservation.UserId == user.Id, "Copy,User,Book");
                    count = await _reservationRepository.GetTotalCountAsync(reservation => reservation.UserId == user.Id);
                    break;
                default:
                    throw new UnauthorizedAccessException("Invalid Role");
            }

            return (reservations, count);
        }

        public async Task<Reservation> GetByIdAsync(int id)
        {
            Reservation reservation = await _reservationRepository.GetAsync(r => r.Id == id, includeProperties: "Copy,User,Book");

            if (reservation == null)
            {
                throw new KeyNotFoundException("No reservation was found");
            }

            return reservation;
        }

        public async Task<Reservation> ReserveAsync(ReservationCreateDto reservationCreate, string jwt)
        {
            await _validator.Validate(reservationCreate);

            string username = GetClaimFromJwt(jwt, "unique_name");
            var user = await _localUserRepository.GetAsync(u => u.UserName == username);

            Reservation model = new()
            {
                ReservationEnd = reservationCreate.ReservationEnd,
                ReservationDate = reservationCreate.ReservationDate,
                UserId = user.Id,
            };

            var book = await _bookRepository.GetAsync(b => b.ISBN == reservationCreate.ISBN);
            model.BookId = book.Id;

            var copy = await _copiesRepository.GetAsync(c => c.IsAvailable && c.BookId == book.Id);
            model.CopyId = copy.Id;

            await _reservationRepository.AddAsync(model);
            await _reservationRepository.SaveAsync();

            await _copiesRepository.MarkAsUnavailable(copy.Id);
            await _copiesRepository.SaveAsync();

            return model;
        }

        public async Task<Reservation> PickUpAsync(int id)
        {
            Reservation reservationFromDb = await _reservationRepository.GetAsync(r => r.Id == id);

            if (reservationFromDb == null)
            {
                throw new KeyNotFoundException("The reservation wasn't found");
            }

            if (reservationFromDb.PickupDate != null)
            {
                throw new ArgumentException("The reservation was already pickup");
            }

            if(reservationFromDb.ReservationEnd < DateTime.UtcNow)
            {
                throw new ArgumentException("The reservation has expired and cannot be picked up");
            }

            if(reservationFromDb.ReservationDate > DateTime.UtcNow)
            {
                throw new ArgumentException("The reservation hasn't started yet");
            }

            await _reservationRepository.NotifyPickup(reservationFromDb);
            await _reservationRepository.SaveAsync();

            return reservationFromDb;
        }

        public async Task<Reservation> ReturnAsync(int id)
        {
            Reservation reservationFromDb = await _reservationRepository.GetAsync(r => r.Id == id);

            if (reservationFromDb == null)
            {
                throw new KeyNotFoundException("The reservation wasn't found");
            }

            if(reservationFromDb.PickupDate == null)
            {
                throw new ArgumentException("The reservation wasn't picked up");
            }

            if (reservationFromDb.ReturnDate != null)
            {
                throw new ArgumentException("The reservation was already returned");
            }

            await _reservationRepository.NotifyReturn(reservationFromDb);;
            await _reservationRepository.SaveAsync();
            await _copiesRepository.MarkAsAvailable(reservationFromDb.CopyId);

            return reservationFromDb;
        }


        private string GetClaimFromJwt(string jwt, string claim)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokens = handler.ReadToken(jwt) as JwtSecurityToken;
            return tokens.Claims.FirstOrDefault(cl => cl.Type == claim).Value.ToString();
        }

        
    }
}
