using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services;
using BookReservationAPI.Services.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace BookReservationAPI.Tests.Services
{
    public class CategoriesServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly ICategoriesService _categoriesService;
        public CategoriesServiceTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _categoriesService = new CategoriesService(_categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task  GetCategoryAsync_ValidId_ReturnsCategory()
        {
            Category category = GetCategory();

            _categoryRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
                .ReturnsAsync(category);

            Category categoryReturn = await _categoriesService.GetCategoryAsync(category.Id);

            Assert.NotNull(categoryReturn);
            Assert.Equal(category, categoryReturn);
        }

        [Fact]
        public async Task GetCategoryAsync_CategoryDoesntExists_ThrowsException()
        {
            _categoryRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>(), It.IsAny<string?>()))
                .ReturnsAsync((Category)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _categoriesService.GetCategoryAsync(5));
        }

        [Fact]
        public async Task CreateAsync_ValidCategory_ReturnsCategory()
        {
            Category category = GetCategory(id: 0);

            _categoryRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _categoryRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            Category categoryReturn = await _categoriesService.CreateAsync(category);

            Assert.NotNull(categoryReturn);
            Assert.Equal(category, categoryReturn);
        }

        [Theory]
        [InlineData(1,"test", "test")]
        [InlineData(0, null, "test")]
        [InlineData(0, "test", null)]
        [InlineData(0, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "test")]
        public async Task CreateAsync_InvalidCategory_ThrowsException(int id, string name, string description)
        {
            Category invalidCategory = GetCategory(id: id, name: name, description: description);

            await Assert.ThrowsAsync<ArgumentException>(async () => await _categoriesService.CreateAsync(invalidCategory));
        }

        [Fact]
        public async Task DeleteAsync_ValidId_DeletesCategory()
        {
            Category category = GetCategory();

            _categoryRepositoryMock.Setup(r => r.Remove(It.IsAny<Category>()));
            _categoryRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            await _categoriesService.DeleteAsync(category);

            _categoryRepositoryMock.Verify(r => r.Remove(category), Times.Once);
            _categoryRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CategoryDoesntExists_ThrowsException()
        {
            _categoryRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync((Category)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _categoriesService.DeleteAsync(1));
        }
        
        [Fact]
        public async Task UpdateAsync_ValidEntity_UpdatesCategory()
        {
            Category category = GetCategory(name: "updated");
            _categoryRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(GetCategory());
            _categoryRepositoryMock.Setup(r => r.Update(It.IsAny<Category>()));
            _categoryRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            await _categoriesService.UpdateAsync(category, category.Id);

            _categoryRepositoryMock.Verify(r => r.Update(category), Times.Once);
            _categoryRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }
        
            private Category GetCategory(int id = 1, string name = "nameTest", string description = "descriptionTest")
        {
            return new Category()
            {
                Id = id,
                Name = name,
                Description = description
            };
        }
    }
}
