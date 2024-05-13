using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace BookReservationAPI.Services
{
    public class BooksService : Service<Book, int>, IBooksService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoriesService _categoriesService;
        public BooksService(IBookRepository bookRepository, ICategoriesService categoriesService) : base(bookRepository)
        {
            _bookRepository = bookRepository;
            _categoriesService = categoriesService;
        }

        public async Task<Book> GetBookByISBNAsync(string ISBN)
        {
            ValidateISBN(ISBN);

            Book bookFromDb = await base.GetAsync(b => b.ISBN == ISBN);

            if (bookFromDb == null)
            {
                throw new KeyNotFoundException(message: "No book was found");
            }

            return bookFromDb;
        }

        public override async Task<Book> CreateAsync(Book book)
        {
            validateISBN(book.ISBN);
            ValidateEntity(book);

            return await base.CreateAsync(book);
        }

        public async Task DeleteAsync(string ISBN)
        {
            Book book = await GetBookByISBNAsync(ISBN);
            await base.DeleteAsync(book);
        }

        public async Task UpdateAsync(Book book, string ISBN)
        {
            if(book.ISBN != ISBN)
            {
                throw new ArgumentException("The ISBN in the model does not match the parameter ISBN.");
            }
            ValidateEntity(book);

            Book dbBook = await _bookRepository.GetAsync(b => b.ISBN.Equals(ISBN));
            book.Id = dbBook.Id;

            _bookRepository.Update(book);
            await _bookRepository.SaveAsync();
        }

        private void ValidateISBN(string ISBN)
        {
            if(!Regex.IsMatch(ISBN, "^[0-9]*$")){
                throw new ArgumentException("Invalid ISBN");
            }
        }

        protected override void ValidateEntity(Book book)
        {
            validateCategoryID(book.IdCategory);
            
            base.ValidateEntity(book);
        }

        private void validateISBN(string ISBN)
        {
            Book dbBook = GetAsync(entity => entity.ISBN == ISBN).GetAwaiter().GetResult();
            if (dbBook != null)
            {
                throw new ArgumentException("The ISBN already exists.");
            }
        }

        private void validateCategoryID(int id)
        {
            _categoriesService.GetCategoryAsync(id).GetAwaiter().GetResult();
        }

    }
}
