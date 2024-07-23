namespace BookReservationAPI.Services.Interfaces
{
    public interface IPhotoUploaderService
    {
        Task<string> AddPhotoAsync(IFormFile photo);
        Task DeletePhoto(string id);
    }
}
