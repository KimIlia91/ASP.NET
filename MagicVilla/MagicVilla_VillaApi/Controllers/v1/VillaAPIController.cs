using AutoMapper;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaApi.Controllers.v1
{
    /// <summary>
    /// ввфывфы
    /// </summary>
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;

        public VillaAPIController(IVillaRepository villaRepository, IMapper mapper)
        {
            _villaRepository = villaRepository;
            _mapper = mapper;
            _response = new();
        }

        /// <summary>
        /// Returns all villas
        /// </summary>
        /// <returns>API Response</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ResponseCache(Duration = 30)] // the cache is stored for 30 seconds
        [ResponseCache(CacheProfileName = "Default30")] // добвил имя профиля для кэша
        public async Task<ActionResult<APIResponse>> GetVillasAsync([FromQuery(Name = "Filter occupancy")]int? occupancy,
            [FromQuery(Name = "Search")]string? search, [FromQuery(Name = "Page size")] int pageSize = 0, 
            [FromQuery(Name = "Page number")]int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> villaList;
                if(occupancy > 0)
                {
                    villaList = await _villaRepository.GetAllAsync(v => v.Occupancy == occupancy, tracked:false, 
                        pageSize:pageSize, pageNumber:pageNumber);
                }
                else
                {
                    villaList = await _villaRepository.GetAllAsync(tracked: false, pageSize: pageSize, pageNumber: pageNumber);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(v => v.Name.ToLower().Contains(search.ToLower()));
                }
                Pagination pagination = new() { PageNumber= pageNumber, PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _response.Result = _mapper.Map<List<VillaDto>>(villaList);
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = new List<string> { ex.Message };
                return _response;
            }
        }

        /// <summary>
        /// Returns the villa by id
        /// </summary>
        /// <remarks>Sample request: GET /Villa</remarks>
        /// <param name="id">Villa ID</param>
        /// <returns>Returnr API Reponse</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid ID</response>
        /// <response code="404">Not found. The villa does not exist</response>
        /// <response code="401">If hte user is Unauthorized</response>
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] // don't save cache and don't save error in cache. Display info everytime and don't call to DB
        public async Task<ActionResult<APIResponse>> GetVillaAsync(int id)
        {
            try
            {
                if (id is 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villa = await _villaRepository.GetAsync(v => v.Id == id);
                if (villa is null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = new List<string> { ex.Message };
                return _response;
            }
        }

        /// <summary>
        /// Create a new villa
        /// </summary>
        /// <param name="createDTO"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> CreateVillaAsync([FromBody] VillaCreateDto createDTO)
        {
            try
            {
                if (await _villaRepository.GetAsync(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Villa has alredy exsist!");
                    return BadRequest(ModelState);
                }
                if (createDTO is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                Villa villa = _mapper.Map<Villa>(createDTO);
                await _villaRepository.CreateAsync(villa);
                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.IsSuccess = true;
                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = new List<string> { ex.Message };
                return _response;
            }
        }

        /// <summary>
        /// Delete a villa by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> DeleteVillaAsync(int id)
        {
            try
            {
                if (id is 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villa = await _villaRepository.GetAsync(v => v.Id == id);
                if (villa is null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                await _villaRepository.RemoveAsync(villa);
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = new List<string> { ex.Message };
                return _response;
            }
        }

        /// <summary>
        /// Update information about villa by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateVillaAsync(int id, [FromBody] VillaUpdateDto updateDTO)
        {
            try
            {
                if (updateDTO is null || id != updateDTO.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                Villa villa = _mapper.Map<Villa>(updateDTO);
                await _villaRepository.UpdateAsync(villa);
                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessage = new List<string> { ex.Message };
                return _response;
            }
        }

        /// <summary>
        /// Update a villa patch method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDto"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (id is 0 || patchDto is null)
                return BadRequest();
            var villa = await _villaRepository.GetAsync(v => v.Id == id, tracked: false);
            if (villa is null)
                return BadRequest();
            VillaUpdateDto villaDTO = _mapper.Map<VillaUpdateDto>(villa);
            patchDto.ApplyTo(villaDTO);
            Villa model = _mapper.Map<Villa>(villaDTO);
            await _villaRepository.UpdateAsync(model);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(model);
        }
    }
}
