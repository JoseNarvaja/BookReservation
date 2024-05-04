using AutoMapper;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using System.Net;

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private APIResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet(Name = "GetCategories")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCategories()
        {
            try
            {
                IEnumerable<Category> categories = await _unitOfWork.Categories.GetAllAsync();
                _response.Result = _mapper.Map<List<CategoryDto>>(categories);
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

        [HttpGet("{id:int}", Name ="GetCategory")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCategory([FromRoute] int id)
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

                Category categoryFromDb = await _unitOfWork.Categories.GetAsync(c => c.Id == id);

                if (categoryFromDb == null)
                {
                    _response.Success = false;
                    _response.Messages.Add("No book was found");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<CategoryDto>(categoryFromDb);
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

        [HttpPost(Name = "CreateCategory")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCategory([FromBody] CategoryCreateDto categoryCreate)
        {
            try
            {
                _response.Success = false;
                if (!ModelState.IsValid || categoryCreate == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Result = ModelState;
                    return BadRequest(_response);
                }

                Category model = _mapper.Map<Category>(categoryCreate);
                await _unitOfWork.Categories.AddAsync(model);
                await _unitOfWork.Save();

                _response.Result = _mapper.Map<CategoryDto>(model);
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetCategory", new { id = model.Id }, _response);
            }
            catch (Exception e)
            {
                _response.Success = false;
                _response.Messages.Add(e.Message);
            }
            return _response;
        }

        [HttpDelete("{id:int}",Name ="DeleteCategory")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteCategory([FromRoute] int id)
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

                Category model = await _unitOfWork.Categories.GetAsync(c => c.Id == id);

                if (model == null)
                {
                    _response.Messages.Add("No category was found");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    NotFound(_response);
                }

                _unitOfWork.Categories.Remove(model);
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

        [HttpPut("{id:int}",Name ="UpdateCategory")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateCategory([FromRoute] int id, [FromBody] CategoryUpdateDto categoryUpdate)
        {
            try
            {
                _response.Success = false;

                if (categoryUpdate == null || categoryUpdate.Id != id || !ModelState.IsValid)
                {
                    _response.Result = ModelState;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Category model = _mapper.Map<Category>(categoryUpdate);
                _unitOfWork.Categories.Update(model);
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
    }
}