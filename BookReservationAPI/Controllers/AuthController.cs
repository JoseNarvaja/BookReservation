using AutoMapper;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IUsersService _service;
        private readonly IMapper _mapper;
        private APIResponse _response;
        public AuthController(IUsersService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
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
                _response.Result = await _service.LoginAsync(model);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
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
                await _service.RegisterAsync(model);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
