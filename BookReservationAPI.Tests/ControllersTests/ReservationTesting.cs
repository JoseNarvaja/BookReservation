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

        public ReservationTesting()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>());
            var mapper = mapperConfig.CreateMapper();
            _validator = new ReservationValidator();
            _unitOfWork = new Mock<IUnitOfWork>();

            _controller = new ReservationController(mapper, _unitOfWork.Object, _validator);
        }

        [Fact]
        public async Task GetReservation_Returns_All_Reservations_To_Admin()
        {
            //Arrange
            var token = new JwtTestBuilder().WithRole(StaticData.RoleAdmin).WithExpiration(60).Build();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Authorization", "Bearer " + token);
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            Mock<IReservationRepository> mockRepoReservation = new Mock<IReservationRepository>();
            List<Reservation> testReservations = new List<Reservation>()
            {
                new Reservation(){Id = 1, ReservationDate = DateTime.Now, ReservationEnd= DateTime.Now.AddDays(3), BookId =1, UserId = "1"},
                new Reservation(){Id = 1, ReservationDate = DateTime.Now, ReservationEnd= DateTime.Now.AddDays(3), BookId =2, UserId = "2"},
            };

            mockRepoReservation.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Reservation, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(testReservations);

            _unitOfWork.Setup(uow => uow.Reservations).Returns(mockRepoReservation.Object);

            //Act
            var response = await _controller.GetReservations();

            APIResponse apiResponse = (APIResponse)((OkObjectResult)response.Result).Value;
            IEnumerable<ReservationDto> reservationResultApi = (IEnumerable<ReservationDto>) apiResponse.Result;
            var reservationDtos = (IEnumerable<ReservationDto>)apiResponse.Result;

            //Assert
            Assert.True(apiResponse.Success);
            Assert.Equal(apiResponse.StatusCode, HttpStatusCode.OK);
            
            Assert.NotNull(reservationDtos);
            Assert.Equal(testReservations.Count, reservationDtos.Count());
        }


    }
}
