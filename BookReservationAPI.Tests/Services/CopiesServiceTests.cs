using BookReservationAPI.Models;
using BookReservationAPI.Repositories.Interfaces;
using BookReservationAPI.Services;
using BookReservationAPI.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookReservationAPI.Tests.Services
{
    public class CopiesServiceTests
    {
        private readonly Mock<ICopiesRepository> _copiesRepositoryMock;
        private readonly ICopiesService _copiesService;
        public CopiesServiceTests()
        {
            _copiesRepositoryMock = new Mock<ICopiesRepository>();
            _copiesService = new CopiesService(_copiesRepositoryMock.Object);
        }

        [Fact]
        public async Task DeleteByBarcodeAsync_ValidBarcode_DeletesCopy()
        {
            Copy copy = new Copy() {IsDeleted = false };
            _copiesRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Copy, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
                .ReturnsAsync(copy);

            await _copiesService.DeleteByBarcodeAsync("1234567891234");

            Assert.True(copy.IsDeleted);
            _copiesRepositoryMock.Verify(r => r.Update(copy), Times.Once);
            _copiesRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteByBarcodeAsync_CopyAlreadyDeleted_ThrowsException()
        {
            Copy copy = new Copy() { IsDeleted = true };

            _copiesRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Copy, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
                .ReturnsAsync(copy);

            await Assert.ThrowsAsync<ArgumentException>(async () => await _copiesService.DeleteByBarcodeAsync("1234567891234"));
        }

        [Fact]
        public async Task DeleteByBarcodeAsync_CopyDoesntExists_ThrowsException()
        {
            _copiesRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Copy, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
                .ReturnsAsync((Copy) null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _copiesService.DeleteByBarcodeAsync("1234567891234"));
        }

        [Theory]
        [InlineData("InvalidBarcode")]
        [InlineData("akdhcnskaicos")]
        [InlineData("123456789123")]
        [InlineData("12345678912354")]
        [InlineData("12345678g12354")]
        public async Task DeleteByBarcodeAsync_InvalidBarcode_ThrowsException(string barcode)
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await _copiesService.DeleteByBarcodeAsync(barcode));
        }

        [Fact]
        public async Task GetByBarcodeAsync_CopyDoesntExists_ThrowsException()
        {
            _copiesRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Copy, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync((Copy)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _copiesService.GetByBarcodeAsync("1234567891234"));
        }

        [Fact]
        public async Task GetByBarcodeAsync_ValidBarcode_ReturnCopy()
        {
            string barcode = "1234567891234";
            Copy copy = new Copy() {Barcode=barcode };
            _copiesRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Copy, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(copy);

            Copy result = await _copiesService.GetByBarcodeAsync(barcode);

            Assert.Equal(barcode, result.Barcode);
            Assert.Equal(result, copy);
        }

        [Fact]
        public async Task UpdateAsync_BarcodeDoesntMatch_ThrowsException()
        {
            string barcode = "2222222222222";
            Copy copy = new Copy() {Barcode = "1111111111111" };

            await Assert.ThrowsAsync<ArgumentException>(async () => await _copiesService.UpdateAsync(barcode, copy));
        }

        [Fact]
        public async Task UpdateAsync_CopyDoesntExist_ThrowsException()
        {
            _copiesRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Copy, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync((Copy)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _copiesService.UpdateAsync("1111111111111", new Copy() { Barcode= "1111111111111" }));
        }

        [Fact]
        public async Task UpdateAsync_ValidRequest_UpdatesCopy()
        {
            string barcode = "1111111111111";
            Copy copy = new Copy() { Barcode = barcode };

            _copiesRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Copy, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
            .ReturnsAsync(copy);

            _copiesRepositoryMock
                .Setup(r => r.Update(It.IsAny<Copy>()));

            _copiesRepositoryMock.Setup(r => r.SaveAsync());

            await _copiesService.UpdateAsync(barcode, copy);

            _copiesRepositoryMock.Verify(r => r.Update(It.IsAny<Copy>()), Times.Once);
            _copiesRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Theory]
        [InlineData(null, false, 1)]
        [InlineData("12345678912", false, 1)]
        [InlineData("ABCDEFGHIJKLM", false, 1)]
        [InlineData("", false, 1)]
        public async Task CreateAsync_InvalidModel_ThrowsException(string barcode, bool isAvailable, int bookId)
        {
            Copy copy = new Copy
            {
                Barcode = barcode,
                IsAvailable = isAvailable,
                BookId = bookId
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => await _copiesService.CreateAsync(copy));
        }

        [Fact]
        public async Task CreateAsync_ValidModel_CreatesNewCopy()
        {
            Copy copy = new Copy
            {
                Barcode = "1234567894561",
                IsAvailable = true,
                BookId = 1
            };

            _copiesRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Copy>()));
            _copiesRepositoryMock.Setup(repo => repo.SaveAsync());

            Copy result = await _copiesService.CreateAsync(copy);

            _copiesRepositoryMock.Verify(r => r.AddAsync(copy), Times.Once);
            _copiesRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);
            Assert.Equal(copy, result);
        }
    }
}
