using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;

namespace BookReservationAPI.Services.Interfaces
{
    public interface ICopiesService : IService<Copy, int>
    {
        Task<Copy> GetByBarcodeAsync(string barcode);
        Task UpdateAsync(string barcode, Copy copy);
        Task DeleteByBarcodeAsync(string barcode);
    }
}
