using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;

namespace BookReservationAPI.Services.Interfaces
{
    public interface IUsersService : IService<LocalUser, string>
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto login);
        Task<UserDto> RegisterAsync(RegisterRequestDto user);
    }
}
