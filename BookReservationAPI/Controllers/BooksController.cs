using AutoMapper;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Models.Pagination;
using BookReservationAPI.Repository.Interfaces;
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
    public class BooksController : BaseController
    {
        private APIResponse _response;
        private readonly IMapper _mapper;
        private IBooksService _booksService;
        private IPhotoUploaderService _photoUploaderService;
        public BooksController(IMapper mapper, IBooksService bookService, IPhotoUploaderService photoUploaderService)
        {
            _response = new APIResponse();
            _booksService = bookService;
            _mapper = mapper;
            _photoUploaderService = photoUploaderService;
        }

        [HttpGet(Name = "GetBooks")]
        [AllowAnonymous]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetBooks([FromQuery] PaginationParams pagination)
        {
            try
            {
                var (books, count) = await _booksService.GetAllWithTotalCountAsync(pagination);

                PaginationHeader paginationHeader = new PaginationHeader(pagination.PageNumber, pagination.PageSize, count);

                Response.AddPaginationHeader(paginationHeader);

                _response.Result = _mapper.Map<IEnumerable<BookDto>>(books);
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

        [HttpPut("{ISBN}/image")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateImage([FromRoute] string ISBN,IFormFile photo)
        {
            try
            {
                Book book = await _booksService.GetBookByISBNAsync(ISBN);

                if(book.ImageUrl != null)
                {
                    await _photoUploaderService.DeletePhoto(book.ImageId);
                }
                
                var (photoUrl, photoPublicId) = await _photoUploaderService.AddPhotoAsync(photo);
                book.ImageUrl = photoUrl;
                book.ImageId = photoPublicId;

                await _booksService.UpdateAsync(book, book.ISBN);

                return NoContent();
            }
            catch (Exception e)
            {
                return this.HandleException(e);
            }
        }

    }
}
