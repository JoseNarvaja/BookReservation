using BookReservationAPI.Models.Dto;

namespace BookReservationAPI.Repository.Interfaces
{
    public interface IUserRepository
    {
        bool IsUnique(string userName);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequest);
        Task<UserDto> Register(RegisterRequestDto registerRequest);
    }
}
