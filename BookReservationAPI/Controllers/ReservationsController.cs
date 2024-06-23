using AutoMapper;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Models.Pagination;
using BookReservationAPI.Services.Interfaces;
using BookReservationAPI.Utility;
using BookReservationAPI.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ReservationsController : BaseController
    {
        private IReservationsService _service;
        private readonly IMapper _mapper;
        private APIResponse _response;
        
        public ReservationsController(IReservationsService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
            _response = new APIResponse();
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetReservations([FromQuery] PaginationParams pagination)
        {
            try
            {
                string jwt = Request.Headers.AsQueryable().FirstOrDefault(x => x.Key == "Authorization").Value.ToString().Replace("Bearer ", "");
                var (reservations, count) = await _service.GetAllWithCountAsync(jwt, pagination);


                PaginationHeader paginationHeader = new PaginationHeader(pagination.PageNumber, pagination.PageSize, count);
                Response.AddPaginationHeader(paginationHeader);

                _response.Success = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<IEnumerable<ReservationDto>>(reservations);

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:int}", Name = "GetReservation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetReservation([FromRoute] int id)
        {
            try
            {
                ReservationDto reservation = _mapper.Map<ReservationDto>(await _service.GetReservationAsync(id));
                _response.Result = reservation;
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<APIResponse>> ReserveBook([FromBody] ReservationCreateDto reservationCreateDto)
        {
            try
            {
                string jwt = Request.Headers.AsQueryable().FirstOrDefault(x => x.Key == "Authorization").Value.ToString().Replace("Bearer ", "");
                Reservation reservation = await _service.ReserveAsync(reservationCreateDto, jwt);

                _response.Result = _mapper.Map<ReservationDto>(reservation);
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetReservation", new {id = reservation.Id}, _response);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:int}/pickup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = StaticData.RoleAdmin)]
        public async Task<ActionResult<APIResponse>> PickupReservation([FromRoute] int id)
        {
            try
            {
                ReservationDto reservation = _mapper.Map<ReservationDto>(await _service.PickUpAsync(id));

                _response.Result = reservation;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success= true;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:int}/return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = StaticData.RoleAdmin)]
        public async Task<ActionResult<APIResponse>> ReturnReservation([FromRoute] int id)
        {
            try
            {
                ReservationDto reservation = _mapper.Map<ReservationDto>(await _service.ReturnReservationAsync(id));

                _response.Result = reservation;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        
    }
}
