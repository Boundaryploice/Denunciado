using AutoMapper;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Application.Services
{
    public class MunicipalityService : IMunicipalityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MunicipalityService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MunicipalityDto>> GetAllMunicipalitiesAsync()
        {
            var municipalities = await _unitOfWork.Municipalities.GetAsync(m => m.IsActive);
            return _mapper.Map<List<MunicipalityDto>>(municipalities);
        }

        public async Task<MunicipalityDto> GetMunicipalityByIdAsync(int id)
        {
            var municipality = await _unitOfWork.Municipalities.GetByIdAsync(id);
            return municipality != null ? _mapper.Map<MunicipalityDto>(municipality) : null;
        }

        public async Task<MunicipalityDto> GetMunicipalityWithDistrictsAsync(int id)
        {
            var municipality = await _unitOfWork.Municipalities.GetMunicipalityWithDistrictsAsync(id);
            if (municipality == null) return null;

            var municipalityDto = _mapper.Map<MunicipalityDto>(municipality);
            municipalityDto.Districts = _mapper.Map<List<DistrictDto>>(municipality.Districts);

            return municipalityDto;
        }

        public async Task<MunicipalityDto> CreateMunicipalityAsync(MunicipalityDto municipalityDto)
        {
            var municipality = _mapper.Map<Municipality>(municipalityDto);
            municipality.CreatedAt = DateTime.UtcNow;
            municipality.IsActive = true;

            await _unitOfWork.Municipalities.AddAsync(municipality);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MunicipalityDto>(municipality);
        }

        public async Task<MunicipalityDto> UpdateMunicipalityAsync(MunicipalityDto municipalityDto)
        {
            var municipality = await _unitOfWork.Municipalities.GetByIdAsync(municipalityDto.Id);
            if (municipality == null) return null;

            _mapper.Map(municipalityDto, municipality);
            municipality.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Municipalities.UpdateAsync(municipality);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MunicipalityDto>(municipality);
        }

        public async Task<bool> DeleteMunicipalityAsync(int id)
        {
            var municipality = await _unitOfWork.Municipalities.GetByIdAsync(id);
            if (municipality == null) return false;

            municipality.IsActive = false;
            municipality.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Municipalities.UpdateAsync(municipality);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
