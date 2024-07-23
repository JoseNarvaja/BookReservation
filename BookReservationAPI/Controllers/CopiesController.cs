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

namespace BookReservationAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CopiesController : BaseController
    {
        private readonly ICopiesService _copiesService;
        private readonly IBooksService _booksService;
        private readonly IMapper _mapper;
        private APIResponse _response;

        public CopiesController(ICopiesService copiesService, IMapper mapper)
        {
            _copiesService = copiesService;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet(Name ="GetCopies")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = StaticData.RoleAdmin)]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCopies([FromQuery] PaginationParams pagination)
        {
            try
            {
                var (copies, count) = await _copiesService.GetAllWithTotalCountAsync(pagination);

                PaginationHeader paginationHeader = new PaginationHeader(pagination.PageNumber, pagination.PageSize, count);
                Response.AddPaginationHeader(paginationHeader);

                _response.Result = _mapper.Map<IEnumerable<CopyDto>>(copies);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{barcode}", Name ="GetCopyByBarcode")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = StaticData.RoleAdmin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCopyByBarcode([FromRoute] string barcode)
        {
            try 
            {
                CopyDto copyDto = _mapper.Map<CopyDto>(await _copiesService.GetByBarcodeAsync(barcode));

                _response.Result = copyDto;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }

        [HttpPost(Name ="CreateCopy")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = StaticData.RoleAdmin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCopy([FromBody] CopyCreateDto copyCreate)
        {
            Copy copyModel;
            try
            {
                Book book = await _booksService.GetBookByISBNAsync(copyCreate.ISBN);
                copyModel = new Copy()
                {
                    IsAvailable = true,
                    Barcode = copyCreate.Barcode,
                    BookId = book.Id
                };

                Copy result = await _copiesService.CreateAsync(copyModel);

                _response.Result = _mapper.Map<CopyDto>(result);
                _response.StatusCode = HttpStatusCode.Created;
                _response.Success = true;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            return CreatedAtRoute("GetCopyByBarcode", new {Barcode = copyModel.Barcode }, _response);
        }

        [HttpPut("{barcode}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = StaticData.RoleAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateByBarcode([FromRoute] string barcode, [FromBody] CopyUpdateDto copyUpdateDto)
        {
            try
            {
                await _copiesService.UpdateAsync(barcode, _mapper.Map<Copy>(copyUpdateDto));
                return NoContent();
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
        }

        [HttpDelete("{barcode}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = StaticData.RoleAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteByBarcode([FromRoute] string barcode)
        {
            try
            {
                await _copiesService.DeleteByBarcodeAsync(barcode);
                return NoContent();
            }
            catch(Exception e)
            {
                return HandleException(e);
            }

        }
    }
}
