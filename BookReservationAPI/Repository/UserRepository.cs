using AutoMapper;
using BookReservationAPI.Data;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookReservationAPI.Utility;

namespace BookReservationAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<LocalUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private string _jwtKey;
        public UserRepository(AppDbContext context, UserManager<LocalUser> userManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _context= context;
            _userManager= userManager;
            _jwtKey = configuration.GetValue<string>("ApiSettings:Secret");
            _roleManager= roleManager;
            _mapper = mapper;
        }
        public bool IsUnique(string userName)
        {
            var user = _context.LocalUsers.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower());
            return user == null ? true : false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequest)
        {
            LocalUser user = _context.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequest.UserName.ToLower());

            bool isPasswordValid =await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (user == null || !isPasswordValid)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            var userRole = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, userRole.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDto>(user)
            };

            return loginResponseDto;


        }

        public async Task<UserDto> Register(RegisterRequestDto registerRequest)
        {
            LocalUser user = new LocalUser()
            {
                UserName= registerRequest.UserName,
                Email= registerRequest.Email,
                Name = registerRequest.Name,
                Surname = registerRequest.Surname,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerRequest.Password);
                if(result.Succeeded)
                {
                    //if(await _roleManager.RoleExistsAsync(StaticData.RoleAdmin))
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole(StaticData.RoleAdmin));
                    //    await _roleManager.CreateAsync(new IdentityRole(StaticData.RoleCustomer));
                    //}

                    await _userManager.AddToRoleAsync(user, StaticData.RoleCustomer);
                    var userToReturn = _context.LocalUsers
                        .FirstOrDefault(u => u.UserName == registerRequest.UserName);
                    return _mapper.Map<UserDto>(userToReturn);
                }
            }
            catch(Exception ex)
            {

            }
            return new UserDto();
        }
    }
}
