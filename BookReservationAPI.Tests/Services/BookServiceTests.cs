using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services;
using BookReservationAPI.Services.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace BookReservationAPI.Tests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepositoryMock;
        private readonly Mock<ICategoriesService> _mockCategoriesServiceMock;
        private readonly IBooksService _booksService;

        public BookServiceTests()
        {
            _mockBookRepositoryMock = new Mock<IBookRepository>();
            _mockCategoriesServiceMock = new Mock<ICategoriesService>();
            _booksService = new BooksService(_mockBookRepositoryMock.Object, _mockCategoriesServiceMock.Object);
        }

        [Fact]
        public async Task GetBookByISBNAsync_BookExists_ReturnsBook()
        {
            string ISBN = "1234567891011";
            Book book = GetBook();

            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(book);

            Book result = await _booksService.GetBookByISBNAsync(ISBN);

            Assert.NotNull(result);
            Assert.Equal(book, result);
        }

        [Fact]
        public async Task GetBookByISBNAsync_BookDoesntExists_ThrowsException()
        {

            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync((Book) null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _booksService.GetBookByISBNAsync("1234567891011"));
        }

        [Theory]
        [InlineData("123456789101122")]
        [InlineData("123456789101")]
        [InlineData("abcdefghijklm")]
        public void GetBookByISBNAsync_ISBNInvalid_ThrowsException(string ISBN)
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _booksService.GetBookByISBNAsync(ISBN));
        }

        [Fact]
        public async void CreateAsync_ValidBook_ReturnsBook()
        {
            Book book = GetBook();

            _mockBookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            _mockBookRepositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync((Book)null);

            _mockCategoriesServiceMock.Setup(cs => cs.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Category { Id = 1, Name = "" });

            Book result = await _booksService.CreateAsync(book);

            Assert.NotNull(result);
            Assert.Equal(book, result);
            _mockBookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public void CreateAsync_ISBNAlreadyExists_ThrowsException()
        {
            Book book = GetBook();

            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(book);

            Assert.ThrowsAsync<ArgumentException>(async () => await _booksService.CreateAsync(book));
        }

        [Theory]
        [InlineData("1234567890123", null, "author", 1)]
        [InlineData("1234567890123", "title", null, 1)]
        [InlineData("1234567890123", "title", null, -1)]
        [InlineData("1234567890123", "title", "author", null)]
        public void CreateAsync_InvalidBook_ThrowsException(string ISBN, string title, string author, int categoryId)
        {
            Book book = GetBook(ISBN, categoryId: categoryId);

            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync((Book)null);

            _mockCategoriesServiceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new Category() {Id = 1, Description= "" });
            _mockCategoriesServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Category)null);

            Assert.ThrowsAsync<ArgumentException>(async () => await _booksService.CreateAsync(book));
        }

        [Fact]
        public async void DeleteAsync_ValidBook_DeletesBook()
        {
            Book book = GetBook();

            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(book);

            _mockBookRepositoryMock.Setup(r => r.Remove(book));
            _mockBookRepositoryMock.Setup(r =>  r.SaveAsync()).Returns(Task.CompletedTask);

            await _booksService.DeleteAsync(book.ISBN);

            _mockBookRepositoryMock.Verify(r => r.Remove(book), Times.Once);
            _mockBookRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);   
        }

        [Fact]
        public void DeleteAsync_BookDoesntExists_ThrowsException()
        {
            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync((Book)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _booksService.DeleteAsync("1234567890123"));
        }

        [Fact]
        public async void UpdateAsync_ValidBook_UpdatesBook()
        {
            string ISBN = "1234567890123";
            Book existingBook = GetBook(ISBN: ISBN);
            Book updatedBook = GetBook(ISBN: ISBN, categoryId: 2);

            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(existingBook);

            _mockBookRepositoryMock.Setup(r => r.Update(updatedBook));

            _mockBookRepositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockCategoriesServiceMock.Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(new Category() {Id= 2, Name= "test" });

            await _booksService.UpdateAsync(updatedBook, ISBN);

            Assert.Equal(updatedBook.Id, existingBook.Id);
            _mockBookRepositoryMock.Verify(r => r.Update(updatedBook), Times.Once);
            _mockBookRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async void UpdateAsync_DifferentISBN_ThrowsException()
        {
            string ISBN = "1234567890123";
            Book book = GetBook(ISBN: "9876543210987");

            await Assert.ThrowsAsync<ArgumentException>(async () => await _booksService.UpdateAsync(book, ISBN));
        }

        [Fact]
        public async void UpdateAsync_NonExistingBook_ThrowsException()
        {
            string ISBN = "1234567890123";
            Book book = GetBook(ISBN: ISBN);

            _mockCategoriesServiceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new Category() { Id = 1, Description = "" });

            _mockBookRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync((Book) null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _booksService.UpdateAsync(book, ISBN));
        }

        private Book GetBook(string ISBN= "1234567890123", string title = "Test Title", string author = "Test Author", int categoryId = 1)
        {
            return new Book
            {
                Title = title,
                Description = "Test Description",
                ISBN = ISBN,
                Author = author,
                ImageUrl = "https://test.com/image.jpg",
                IdCategory = categoryId, 
                //Stock = 10
            };
        }

    }
}
