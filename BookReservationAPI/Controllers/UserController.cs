using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private APIResponse _response;
        public UserController(IUserRepository userRepository)
        {
            _userRepository= userRepository;
            _response = new APIResponse();
        }

        [HttpPost("login")]
        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestDto model)
        {
            LoginResponseDto loginResponse = await _userRepository.Login(model);
            if(loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Success= false;
                _response.Messages.Add("The password or username is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode= HttpStatusCode.OK;
            _response.Success = true;
            _response.Result= loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<APIResponse>> Register([FromBody] RegisterRequestDto model)
        {
            bool usernameUnique = _userRepository.IsUnique(model.UserName);
            if(!usernameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Success= false;
                _response.Messages.Add("The username is already taken");
                return BadRequest(_response);
            }

            var user = await _userRepository.Register(model);
            if(user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Success= false;
                _response.Messages.Add("An error ocurred during registration");
            }

            _response.StatusCode= HttpStatusCode.OK;
            _response.Success= true;
            return Ok(_response);  
        }

    }
}
