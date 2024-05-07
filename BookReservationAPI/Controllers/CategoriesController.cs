using AutoMapper;
using BookReservationAPI.Exceptions;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
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
    public class CategoriesController : ControllerBase
    {
        private APIResponse _response;
        private readonly IMapper _mapper;
        private readonly ICategoriesService _service;
        public CategoriesController(ICategoriesService service, IMapper mapper)
        {
            _service = service;
            _response = new APIResponse();
            _mapper = mapper;
        }

        [HttpGet(Name = "GetCategories")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCategories(int pageSize = 5, int pageNumber = 1)
        {
            try
            {
                IEnumerable<CategoryDto> categories = _mapper.Map<List<CategoryDto>>( await _service.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber));

                Pagination pagination = new Pagination() {PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = categories;
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
            return Ok(_response);
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
                CategoryDto category = _mapper.Map<CategoryDto>(await _service.GetAsync(c => c.Id == id));

                _response.Result = category;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
            return Ok(_response);
        }

        [HttpPost(Name = "CreateCategory")]
        [Authorize(Roles = StaticData.RoleAdmin, AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCategory([FromBody] CategoryCreateDto categoryCreate)
        {
            Category modelCategory;
            try
            {
                modelCategory = _mapper.Map<Category>(categoryCreate);
                _response.Result = _mapper.Map<CategoryDto>(await _service.CreateAsync(modelCategory));
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
            return CreatedAtRoute("GetCategory", new { id = modelCategory.Id }, _response);
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
                await _service.DeleteAsync(id);
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
            return NoContent();
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
                Category model = _mapper.Map<Category>(categoryUpdate);
                await _service.UpdateAsync(model, id);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
            return NoContent();
        }

        private ActionResult HandleException(Exception e)
        {
            HttpStatusCode statusCode;
            string message;

            switch (e)
            {
                case ArgumentException argumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = argumentException.Message;
                    break;
                case KeyNotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = e.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An internal error has occurred. Please try again later.";
                    break;
            }

            throw new BusinessException(statusCode, message);
        }
    }
}