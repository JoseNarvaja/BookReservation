using BookReservationAPI.Services.Interfaces;
using BookReservationAPI.Utility;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace BookReservationAPI.Services
{
    public class PhotoUploaderService : IPhotoUploaderService
    {
        private readonly Cloudinary _cloudinaryService;

        public PhotoUploaderService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.Cloud,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );

            _cloudinaryService = new Cloudinary(account);
        }

        public async Task<string> AddPhotoAsync(IFormFile photo)
        {
            var uploadResult = new ImageUploadResult();

            using var stream = photo.OpenReadStream();
            var imageParams = new ImageUploadParams
            {
                File = new FileDescription(photo.FileName, stream),
                Transformation = new Transformation().Height(600).Width(600).Crop("fill").Gravity("face"),
                Folder = "bookReservation"
            };

            uploadResult = await _cloudinaryService.UploadAsync(imageParams);

            if(uploadResult.Error != null)
            {
                throw new Exception("An unexpected error occurred while uploading the image");
            }

            return uploadResult.SecureUrl.AbsoluteUri;

        }

        public async Task DeletePhoto(string id)
        {
            DeletionParams deletionParams = new DeletionParams(id);
            var result = await _cloudinaryService.DestroyAsync(deletionParams);

            if(result.Error != null)
            {
                throw new Exception("An unexpected error occurred while deleting the image");
            }
        }
    }
}
