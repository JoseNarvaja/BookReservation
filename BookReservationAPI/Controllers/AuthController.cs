using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private APIResponse _response;
        public AuthController(IUserRepository userRepository)
        {
            _userRepository= userRepository;
            _response = new APIResponse();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                LoginResponseDto loginResponse = await _userRepository.Login(model);
                if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Success = false;
                    _response.Messages.Add("The password or username is incorrect");
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                _response.Result = loginResponse;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Success= false;
                _response.Messages.Add("An error occurred while processing your request");
                return _response;
            }
            
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Register([FromBody] RegisterRequestDto model)
        {
            try
            {
                bool usernameUnique = _userRepository.IsUnique(model.UserName);
                if (!usernameUnique)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Success = false;
                    _response.Messages.Add("The username is already taken");
                    return BadRequest(_response);
                }

                var user = await _userRepository.Register(model);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Success = false;
                    _response.Messages.Add("An error ocurred during registration");
                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                _response.Result= user;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Success = false;
                _response.Messages.Add("An error occurred while processing your request");
                return _response;
            }
        }
    }
}
