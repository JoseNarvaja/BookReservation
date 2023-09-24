using AutoMapper;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Utility;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ReservationController : Controller
    {
        private APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationValidator _reservationValidator;
        public ReservationController(IMapper mapper, IUnitOfWork unitOfWork, IReservationValidator reservationValidator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _reservationValidator = reservationValidator;
            _response = new APIResponse();
        }
        [HttpGet("GetReservations")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetReservations()
        {
            try
            {
                string jwt = Request.Headers.AsQueryable().FirstOrDefault(x => x.Key == "Authorization").Value.ToString().Replace("Bearer ", "");
                IEnumerable<ReservationDto> reservations = await GetReservationsForUser(jwt);

                _response.Success = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = reservations;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Messages.Add(ex.Message);
                return _response;
            }
        }

        [HttpPost("ReserveBook")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<APIResponse>> ReserveBook([FromBody] ReservationCreateDto reservationCreateDto)
        {
            _response.Success = false;
            try
            {
                if(!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Result = ModelState;
                    return BadRequest(_response);
                }

                var validationResult = await _reservationValidator.Validate(_unitOfWork, reservationCreateDto);

                if (! validationResult.success)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    foreach(string error in validationResult.Errors)
                    {
                        _response.Messages.Add(error);
                    }

                    return BadRequest(_response);
                }

                Reservation model = _mapper.Map<Reservation>(reservationCreateDto);

                var book = await _unitOfWork.Books.GetAsync(b => b.ISBN == reservationCreateDto.ISBN);
                var user = await _unitOfWork.LocalUsers.GetAsync(u => u.UserName == reservationCreateDto.UserName);
                model.UserId = user.Id;
                model.BookId= book.Id;

                await _unitOfWork.Reservations.AddAsync(model);
                await _unitOfWork.Books.DecreaseCount(reservationCreateDto.ISBN, 1);
                await _unitOfWork.Save();

                _response.Result = _mapper.Map<ReservationDto>(model);
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.Created;

                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.Success = false;
                _response.StatusCode= HttpStatusCode.InternalServerError;
                _response.Messages.Add(ex.Message);
                return _response;
            }
        }

        private async Task<IEnumerable<ReservationDto>> GetReservationsForUser(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokens = handler.ReadToken(jwt) as JwtSecurityToken;
            string role = tokens.Claims.FirstOrDefault(claim => claim.Type == "role").Value.ToString();

            IEnumerable<ReservationDto> reservations;
            switch(role)
            {
                case StaticData.RoleAdmin:
                    reservations = _mapper.Map<List<ReservationDto>>(await _unitOfWork.Reservations.GetAllAsync());
                    break;
                case StaticData.RoleCustomer:
                    HttpContext httpContext = HttpContext;
                    string username = httpContext.User.Identity.Name;
                    var user = await _unitOfWork.LocalUsers.GetAsync(u => u.UserName == username);
                    reservations = _mapper.Map<List<ReservationDto>>(await _unitOfWork.Reservations.GetAllAsync(reservation => reservation.UserId == user.Id));
                    break;
                default:
                    reservations = null;
                    break;
            }

            return reservations;
        }
    }
}
