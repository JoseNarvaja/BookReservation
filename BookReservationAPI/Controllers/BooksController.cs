using AutoMapper;
using BookReservationAPI.Exceptions;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Services.Interfaces;
using BookReservationAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BooksController : BaseController
    {
        private APIResponse _response;
        private readonly IMapper _mapper;
        private IBooksService _booksService;
        public BooksController(IUnitOfWork unitOfWork, IMapper mapper, IBooksService bookService)
        {
            _response = new APIResponse();
            _booksService = bookService;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetBooks")]
        [AllowAnonymous]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetBooks(int pageSize = 5, int pageNumber = 1)
        {
            try
            {
                IEnumerable<BookDto> books = _mapper.Map<IEnumerable<BookDto>>(await _booksService.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber));

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = books;
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
        }

        [HttpGet("{ISBN}", Name = "GetBook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetBook([FromRoute] string ISBN)
        {
            try
            {
                Book bookFromDb = await _booksService.GetBookByISBNAsync(ISBN);

                _response.Result = _mapper.Map<BookDto>(bookFromDb);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
        }

        [HttpPost]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateBook([FromBody] BookCreateDto bookCreate)
        {
            try
            {
                Book model = _mapper.Map<Book>(bookCreate);

                _response.Result = _mapper.Map<BookDto>(await _booksService.CreateAsync(model));
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetBook", new { ISBN = model.ISBN }, _response);
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
        }

        [HttpDelete("{ISBN}", Name = "DeleteBook")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteBook([FromRoute] string ISBN)
        {
            try
            {
                await _booksService.DeleteAsync(ISBN);
                return NoContent();
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
        }

        [HttpPut("{ISBN}", Name ="UpdateBook")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateBook([FromRoute] string ISBN, [FromBody] BookUpdateDto bookUpdate)
        {
            try
            {
                Book bookModel = _mapper.Map<Book>(bookUpdate);
                await _booksService.UpdateAsync(bookModel, ISBN);
                return NoContent();
            }
            catch(Exception e)
            {
                return HandleException(e);
            }
        }

        
    }
}
