using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class DistrictsController : BaseApiController
    {
        private readonly IDistrictService _districtService;

        public DistrictsController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<DistrictDto>>> GetAllDistricts()
        {
            var result = await _districtService.GetAllDistrictsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DistrictDto>> GetDistrictById(int id)
        {
            var result = await _districtService.GetDistrictByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "District not found" });
            }

            return Ok(result);
        }

        [HttpGet("municipality/{municipalityId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<DistrictDto>>> GetDistrictsByMunicipality(int municipalityId)
        {
            var result = await _districtService.GetDistrictsByMunicipalityAsync(municipalityId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DistrictDto>> CreateDistrict(DistrictDto districtDto)
        {
            var result = await _districtService.CreateDistrictAsync(districtDto);

            return CreatedAtAction(nameof(GetDistrictById), new { id = result.Id }, result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DistrictDto>> UpdateDistrict(DistrictDto districtDto)
        {
            var result = await _districtService.UpdateDistrictAsync(districtDto);

            if (result == null)
            {
                return BadRequest(new { message = "District update failed" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDistrict(int id)
        {
            var result = await _districtService.DeleteDistrictAsync(id);

            if (!result)
            {
                return BadRequest(new { message = "District deletion failed" });
            }

            return Ok(new { message = "District deleted successfully" });
        }
    }
}
