using AutoMapper;
using BookReservationAPI.Controllers;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Tests.Jwt;
using BookReservationAPI.Utility;
using BookReservationAPI.Utility.ReservationValidation;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace BookReservationAPI.Tests.Controllers
{
    public class ReservationTesting : EndToEndTestCase
    {
        private readonly ReservationController _controller;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly IReservationValidator _validator;
        private readonly IMapper _mapper;
        private APIResponse _response;
        private List<Reservation> _reservations;
        private List<LocalUser> _users;

        public ReservationTesting()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>());
            var mapper = mapperConfig.CreateMapper();
            _validator = new ReservationValidator();
            _unitOfWork = new Mock<IUnitOfWork>();

            _controller = new ReservationController(mapper, _unitOfWork.Object, _validator);

            Mock<IReservationRepository> mockRepoReservation = new Mock<IReservationRepository>();
            _reservations = new List<Reservation>()
            {
                new Reservation(){Id = 1, ReservationDate = DateTime.Now, ReservationEnd= DateTime.Now.AddDays(3), BookId =1, UserId = "1"},
                new Reservation(){Id = 2, ReservationDate = DateTime.Now, ReservationEnd= DateTime.Now.AddDays(3), BookId =1, UserId = "1"},
                new Reservation(){Id = 3, ReservationDate = DateTime.Now, ReservationEnd= DateTime.Now.AddDays(3), BookId =2, UserId = "2"},
            };

            mockRepoReservation.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Reservation, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<Reservation, bool>> filter, string includeProperties) =>
                {
                    if (filter == null)
                    {
                        return _reservations.ToList();
                    }
                    else
                    {
                        var filteredReservations = _reservations.Where(filter.Compile());
                        return filteredReservations.ToList();
                    }
                });

            _unitOfWork.Setup(uow => uow.Reservations).Returns(mockRepoReservation.Object);

            Mock<ILocalUserRepository> mockRepoLocalUser = new Mock<ILocalUserRepository>();
            _users = new List<LocalUser>()
            {
                new LocalUser(){Id = "1", UserName="1"},
                new LocalUser(){Id = "2", UserName = "2"}
            };

            mockRepoLocalUser.Setup(r => r.GetAsync(It.IsAny<Expression<Func<LocalUser, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns((Expression<Func<LocalUser, bool>> filter, bool tracked, string includeProperties) =>
                {
                    var user = _users.Where(filter.Compile());
                    return Task.FromResult(user.FirstOrDefault());
                });

            _unitOfWork.Setup(uow => uow.LocalUsers).Returns(mockRepoLocalUser.Object);

        }

        [Fact]
        public async Task GetReservations_Returns_All_Reservations_To_Admin()
        {
            //Arrange
            var token = new JwtTestBuilder().WithRole(StaticData.RoleAdmin).WithExpiration(60).Build();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Authorization", "Bearer " + token);
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            //Act
            var response = await _controller.GetReservations();

            APIResponse apiResponse = (APIResponse)((OkObjectResult)response.Result).Value;
            IEnumerable<ReservationDto> reservationResultApi = (IEnumerable<ReservationDto>)apiResponse.Result;
            var reservationDtos = (IEnumerable<ReservationDto>)apiResponse.Result;

            //Assert
            Assert.True(apiResponse.Success);
            Assert.Equal(apiResponse.StatusCode, HttpStatusCode.OK);

            Assert.NotNull(reservationDtos);
            Assert.Equal(_reservations.Count, reservationDtos.Count());
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        public async Task GetReservations_Returns_Only_User_Reservations_To_Customer(string username)
        {
            //Arrange
            var token = new JwtTestBuilder().WithRole(StaticData.RoleCustomer).WithUserName(username).WithExpiration(60).Build();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Authorization", "Bearer " + token);
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            //Act
            var response = await _controller.GetReservations();
            APIResponse apiResponse = (APIResponse)((OkObjectResult)response.Result).Value;
            var reservationDtos = (IEnumerable<ReservationDto>)apiResponse.Result;
            Assert.True(apiResponse.Success);
            Assert.Equal(apiResponse.StatusCode, HttpStatusCode.OK);

            Assert.NotNull(reservationDtos);

            var reservationsFromUser = _reservations.Where(r => r.UserId == username);
            Assert.Equal(reservationDtos.Count(), reservationsFromUser.Count());
        }

    }
}
