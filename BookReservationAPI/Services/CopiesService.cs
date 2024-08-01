using BookReservationAPI.Models;
using BookReservationAPI.Models.Pagination;
using BookReservationAPI.Repositories.Interfaces;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace BookReservationAPI.Services
{
    public class CopiesService : Service<Copy, int>, ICopiesService
    {
        private readonly ICopiesRepository _copiesRepository;
        public CopiesService(ICopiesRepository copiesRepository) : base(copiesRepository)
        {
            _copiesRepository = copiesRepository;
        }

        public async Task DeleteByBarcodeAsync(string barcode)
        {
            ValidateBarcode(barcode);

            Copy copy = await _copiesRepository.GetAsync(c => c.Barcode == barcode);

            if(copy is null)
            {
                throw new KeyNotFoundException("No copy was found");
            }
            if(copy.IsDeleted == true)
            {
                throw new ArgumentException("The copy is already deleted");
            }

            copy.IsDeleted = true;
            _copiesRepository.Update(copy);
            await _copiesRepository.SaveAsync();
        }

        public async Task<Copy> GetByBarcodeAsync(string barcode)
        {
            ValidateBarcode(barcode);

            Copy copy = await this.GetAsync(c => c.Barcode == barcode);
            
            if(copy is null)
            {
                throw new KeyNotFoundException("No copy was found");
            }

            return copy;
        }

        public async Task UpdateAsync(string barcode, Copy copy)
        {
            ValidateBarcode(barcode);

            if(barcode != copy.Barcode)
            {
                throw new ArgumentException("The Barcode in the model does not match the parameter Barcode.");
            }

            Copy dbCopy = await _copiesRepository.GetAsync(c => c.Barcode == barcode);

            if(dbCopy is null)
            {
                throw new KeyNotFoundException("No copy was found");
            }

            dbCopy.BookId = copy.BookId;
            dbCopy.Barcode = copy.Barcode;

            _copiesRepository.Update(dbCopy);
            await _copiesRepository.SaveAsync();
        }

        public override async Task<Copy> CreateAsync(Copy copy)
        {
            ValidateEntity(copy);
            return await base.CreateAsync(copy);
        }

            private void ValidateBarcode(string barcode)
        {
            if (!Regex.IsMatch(barcode, @"^\d{13}$"))
            {
                throw new ArgumentException("Invalid Barcode");
            }
        }
    }
}
