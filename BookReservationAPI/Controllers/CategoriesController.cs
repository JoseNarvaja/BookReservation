using AutoMapper;
using BookReservationAPI.Exceptions;
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
    public class CategoriesController : BaseController
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
        public async Task<ActionResult<APIResponse>> GetCategories([FromQuery] PaginationParams pagination)
        {
            try
            {
                var (categories, count) =  await _service.GetAllWithTotalCountAsync(pagination);

                PaginationHeader paginationHeader = new PaginationHeader(pagination.PageNumber, pagination.PageSize, count);

                Response.AddPaginationHeader(paginationHeader);

                _response.Result = _mapper.Map<List<CategoryDto>>(categories);
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
                CategoryDto category = _mapper.Map<CategoryDto>(await _service.GetCategoryAsync(id));

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
    }
}