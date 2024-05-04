using AutoMapper;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BooksController : ControllerBase
    {
        private APIResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BooksController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _response = new APIResponse();
            _mapper = mapper;
        }
        [HttpGet(Name = "GetBooks")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetBooks()
        {
            try
            {
                IEnumerable<Book> books = await _unitOfWork.Books.GetAllAsync();
                _response.Result = _mapper.Map<List<BookDto>>(books);
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.Success = false;
                _response.Messages.Add(e.Message);
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetBook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetBook([FromRoute] int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.Success = false;
                    _response.Messages.Add("The id must be a correct value");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Book bookFromDb = await _unitOfWork.Books.GetAsync(b => b.Id == id);

                if (bookFromDb == null)
                {
                    _response.Success = false;
                    _response.Messages.Add("No book was found");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<BookDto>(bookFromDb);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.Success = false;
                _response.Messages.Add(e.Message);
            }
            return _response;
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
                _response.Success = false;
                if (!ModelState.IsValid || bookCreate == null || bookCreate.ISBN.Length != 14)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Result = ModelState;
                    return BadRequest(_response);
                }
                if (await _unitOfWork.Books.GetAsync(b => b.ISBN == bookCreate.ISBN) != null)
                {
                    ModelState.AddModelError("Messages", "The ISBN already exists");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Result = ModelState;
                    return BadRequest(_response);
                }

                Book model = _mapper.Map<Book>(bookCreate);
                await _unitOfWork.Books.AddAsync(model);
                await _unitOfWork.Save();

                _response.Result = _mapper.Map<BookDto>(model);
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetBook", new { id = model.Id }, _response);
            }
            catch (Exception e)
            {
                _response.Success = false;
                _response.Messages.Add(e.Message);
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteBook")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteBook([FromRoute] int id)
        {
            try
            {
                _response.Success = false;
                if (id <= 0)
                {
                    _response.Messages.Add("The id must be a correct value");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Book model = await _unitOfWork.Books.GetAsync(b => b.Id == id);

                if (model == null)
                {
                    _response.Messages.Add("No book was found");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _unitOfWork.Books.Remove(model);
                await _unitOfWork.Save();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.Success = true;

                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Messages.Add(e.Message);
            }
            return _response;
        }

        [HttpPut("{id:int}", Name ="UpdateBook")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateBook([FromRoute] int id, [FromBody] BookUpdateDto bookUpdate)
        {
            try
            {
                _response.Success = false;

                if(bookUpdate == null || bookUpdate.Id != id || !ModelState.IsValid)
                {
                    _response.Result = ModelState;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Book model = _mapper.Map<Book>(bookUpdate);
                _unitOfWork.Books.Update(model);
                await _unitOfWork.Save();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.Success = true;
                return Ok(_response);
            }
            catch(Exception e)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Messages.Add(e.Message);
            }
            return _response;
        }
    }
}
