using BookReservationAPI.Controllers;
using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookReservationTests.Controllers
{
    public class BookControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly BookController _controller;
        private APIResponse _response;

        public BookControllerTests()
        {

        }

        [Fact]
        public void GetBooks_ShouldReturnAllBooks()
        {

        }

    }
}
