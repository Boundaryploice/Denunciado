using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    public class MunicipalitiesController : BaseApiController
    {
        private readonly IMunicipalityService _municipalityService;

        public MunicipalitiesController(IMunicipalityService municipalityService)
        {
            _municipalityService = municipalityService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<MunicipalityDto>>> GetAllMunicipalities()
        {
            var result = await _municipalityService.GetAllMunicipalitiesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<MunicipalityDto>> GetMunicipalityById(int id)
        {
            var result = await _municipalityService.GetMunicipalityByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Municipality not found" });
            }

            return Ok(result);
        }

        [HttpGet("{id}/districts")]
        [AllowAnonymous]
        public async Task<ActionResult<MunicipalityDto>> GetMunicipalityWithDistricts(int id)
        {
            var result = await _municipalityService.GetMunicipalityWithDistrictsAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Municipality not found" });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MunicipalityDto>> CreateMunicipality(MunicipalityDto municipalityDto)
        {
            var result = await _municipalityService.CreateMunicipalityAsync(municipalityDto);

            return CreatedAtAction(nameof(GetMunicipalityById), new { id = result.Id }, result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MunicipalityDto>> UpdateMunicipality(MunicipalityDto municipalityDto)
        {
            var result = await _municipalityService.UpdateMunicipalityAsync(municipalityDto);

            if (result == null)
            {
                return BadRequest(new { message = "Municipality update failed" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMunicipality(int id)
        {
            var result = await _municipalityService.DeleteMunicipalityAsync(id);

            if (!result)
            {
                return BadRequest(new { message = "Municipality deletion failed" });
            }

            return Ok(new { message = "Municipality deleted successfully" });
        }
    }
}
