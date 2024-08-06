using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Models.Pagination;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace BookReservationAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _repository;
        public UsersService(IUserRepository repository)
        {
            _repository = repository;
        }

        public Task<LocalUser> CreateAsync(LocalUser entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(LocalUser entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LocalUser>> GetAllAsync(int pageSize = 5, int pageNumber = 1)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<LocalUser>, int)> GetAllWithTotalCountAsync(PaginationParams pagination,Expression<Func<LocalUser, bool>>? filter, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public Task<LocalUser> GetAsync(Expression<Func<LocalUser, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login)
        {
            LoginResponseDto loginResponse = await _repository.Login(login);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                throw new ArgumentException("The password or username is invalid");
            }
            return loginResponse;
        }

        public async Task<UserDto> RegisterAsync(RegisterRequestDto userRegister)
        {
            bool usernameUnique = _repository.IsUnique(userRegister.UserName);
            if (!usernameUnique)
            {
                throw new ArgumentException("The username is already taken");
            }

            var user = await _repository.Register(userRegister);
            if (user == null)
            {
                throw new Exception("An error ocurred during registration");
            }
            return user;
        }
    }
}
