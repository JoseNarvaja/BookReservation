﻿using BookReservationAPI.Models;
using BookReservationAPI.Models.Pagination;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services;
using BookReservationAPI.Services.Interfaces;
using BookReservationAPI.Tests.Jwt;
using BookReservationAPI.Utility;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;
using Moq;
using System.Linq.Expressions;
using BookReservationAPI.Repositories.Interfaces;

namespace BookReservationAPI.Tests.Services
{
    public class ReservationsServiceTests
    {
        private readonly IReservationsService _reservationsService;
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<ILocalUserRepository> _localUserRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IReservationValidator> _reservationValidatorMock;
        private readonly Mock<ICopiesRepository> _copiesRepositoryMock;
        private readonly PaginationParams _paginationParams;
        public ReservationsServiceTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _localUserRepositoryMock = new Mock<ILocalUserRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _reservationValidatorMock = new Mock<IReservationValidator>();
            _copiesRepositoryMock = new Mock<ICopiesRepository>();
            _reservationsService = new ReservationsService(_reservationRepositoryMock.Object,_localUserRepositoryMock.Object,
                _reservationValidatorMock.Object, _bookRepositoryMock.Object, _copiesRepositoryMock.Object);
            _paginationParams = new PaginationParams();
        }

        [Fact]
        public async void GetAllAsync_AdminRole_ReturnsAllReservations()
        {
            string jwt = new JwtTestBuilder().WithRole(StaticData.RoleAdmin).Build();
            IEnumerable <Reservation> reservations = GetReservations();

            _reservationRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<PaginationParams>(), It.IsAny<Expression<Func<Reservation, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync((reservations));

            var (result, count) = await _reservationsService.GetAllWithCountAsync(jwt, new PaginationParams());

            Assert.NotNull(result);
            Assert.Equal(result.Count(), reservations.Count());
        }

        [Fact]
        public async void GetAllAsync_CustomerRole_ReturnsUserReservations()
        {
            string jwt = new JwtTestBuilder().WithRole(StaticData.RoleCustomer).WithUserName("username").Build();
            IEnumerable<Reservation> reservations = GetReservations();
            LocalUser user = new LocalUser {UserName = "username", Id = "1" };

            _localUserRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<LocalUser, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(user);

            _reservationRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<PaginationParams>(),It.IsAny<Expression<Func<Reservation, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(reservations.Where(r => r.UserId == user.Id));

            var (result, count) = await _reservationsService.GetAllWithCountAsync(jwt, _paginationParams);

            Assert.NotNull(result);
            Assert.Equal(result.Count(), reservations.Where(r => r.UserId == user.Id).Count());
            _localUserRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<LocalUser, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()) ,Times.Once);
        }

        [Fact]
        public async void GetAllAsync_InvalidRole_ThrowsException()
        {
            string jwt = new JwtTestBuilder().WithRole("InvalidRole").Build();

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _reservationsService.GetAllWithCountAsync(jwt, _paginationParams));
        }

        [Fact]
        public async void GetReservationAsync_ValidId_ReturnsReservation()
        {
            int validId = 1;
            Reservation reservation = GetReservations().Where(r => r.Id == validId).First();

            _reservationRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Reservation, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(reservation);

            Reservation result = await _reservationsService.GetByIdAsync(validId);

            Assert.Equal(reservation, result);
            _reservationRepositoryMock.Verify(r =>  r.GetAsync(It.IsAny<Expression<Func<Reservation, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void GetReservationAsync_InvalidId_ThrowsException()
        {
            int InvalidId = 5;

            _reservationRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Reservation, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync((Reservation)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _reservationsService.GetByIdAsync(InvalidId));

        }

        [Fact]
        public async void ReserveAsync_ValidReservation_ReturnsReservation()
        {
            LocalUser user = new LocalUser {UserName =  "test" };
            string jwt = new JwtTestBuilder().WithUserName(user.UserName).Build();
            Book book = new Book() {Id = 1, ISBN = "1234567891234" };
            Copy copy = new Copy() {Id = 1, Barcode = "1234512452169", BookId=1, IsAvailable=true, IsDeleted=false };

            _reservationValidatorMock.Setup(v => v.Validate(It.IsAny<ReservationCreateDto>()));
            _localUserRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<LocalUser, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(user);
            _bookRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(book);
            _copiesRepositoryMock
                .Setup(c => c.GetAsync(It.IsAny<Expression<Func<Copy, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(copy);
            _reservationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Reservation>()));
            _reservationRepositoryMock.Setup(r => r.SaveAsync());

            ReservationCreateDto reservationDto = new ReservationCreateDto
            {
                ISBN = book.ISBN,
                ReservationDate = DateTime.Now.AddDays(1),
                ReservationEnd = DateTime.Now.AddDays(7),
            };

            Reservation result = await _reservationsService.ReserveAsync(reservationDto, jwt);

            Assert.NotNull(result);
            Assert.Equal(reservationDto.ReservationDate, result.ReservationDate);
            Assert.Equal(reservationDto.ReservationEnd, result.ReservationEnd);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(book.Id, result.BookId);
            Assert.Equal(copy.Id, result.CopyId);

        }

        private IEnumerable<Reservation> GetReservations()
        {
            return new List<Reservation>
            {
                new Reservation {Id = 1, UserId = "1", BookId = 1 },
                new Reservation {Id = 2, UserId = "2", BookId = 2 },
                new Reservation {Id = 3, UserId = "1", BookId = 3 }
            };
        }
    }
}
