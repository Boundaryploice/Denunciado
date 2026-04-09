using AutoMapper;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Application.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DistrictService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<DistrictDto>> GetAllDistrictsAsync()
        {
            var districts = await _unitOfWork.Districts.GetAsync(
                d => d.IsActive,
                null,
                "Municipality");
            return _mapper.Map<List<DistrictDto>>(districts);
        }

        public async Task<DistrictDto> GetDistrictByIdAsync(int id)
        {
            var district = await _unitOfWork.Districts.GetByIdAsync(id);
            return district != null ? _mapper.Map<DistrictDto>(district) : null;
        }

        public async Task<List<DistrictDto>> GetDistrictsByMunicipalityAsync(int municipalityId)
        {
            var districts = await _unitOfWork.Districts.GetDistrictsByMunicipalityAsync(municipalityId);
            return _mapper.Map<List<DistrictDto>>(districts);
        }

        public async Task<DistrictDto> CreateDistrictAsync(DistrictDto districtDto)
        {
            var district = _mapper.Map<District>(districtDto);
            district.CreatedAt = DateTime.UtcNow;
            district.IsActive = true;

            await _unitOfWork.Districts.AddAsync(district);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<DistrictDto>(district);
        }

        public async Task<DistrictDto> UpdateDistrictAsync(DistrictDto districtDto)
        {
            var district = await _unitOfWork.Districts.GetByIdAsync(districtDto.Id);
            if (district == null) return null;

            _mapper.Map(districtDto, district);
            district.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Districts.UpdateAsync(district);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<DistrictDto>(district);
        }

        public async Task<bool> DeleteDistrictAsync(int id)
        {
            var district = await _unitOfWork.Districts.GetByIdAsync(id);
            if (district == null) return false;

            district.IsActive = false;
            district.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Districts.UpdateAsync(district);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
