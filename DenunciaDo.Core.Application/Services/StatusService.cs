using AutoMapper;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Core.Domain.Interfaces.Services;

namespace DenunciaDo.Application.Services
{
    public class StatusService : IStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<StatusDto>> GetAllStatusesAsync()
        {
            var statuses = await _unitOfWork.Statuses.GetAsync(s => s.IsActive);
            return _mapper.Map<List<StatusDto>>(statuses);
        }

        public async Task<StatusDto> GetStatusByIdAsync(int id)
        {
            var status = await _unitOfWork.Statuses.GetByIdAsync(id);
            return status != null ? _mapper.Map<StatusDto>(status) : null;
        }

        public async Task<StatusDto> CreateStatusAsync(StatusDto statusDto)
        {
            var status = _mapper.Map<Status>(statusDto);
            status.CreatedAt = DateTime.UtcNow;
            status.IsActive = true;

            await _unitOfWork.Statuses.AddAsync(status);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StatusDto>(status);
        }

        public async Task<StatusDto> UpdateStatusAsync(StatusDto statusDto)
        {
            var status = await _unitOfWork.Statuses.GetByIdAsync(statusDto.Id);
            if (status == null) return null;

            _mapper.Map(statusDto, status);
            status.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Statuses.UpdateAsync(status);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StatusDto>(status);
        }

        public async Task<bool> DeleteStatusAsync(int id)
        {
            var status = await _unitOfWork.Statuses.GetByIdAsync(id);
            if (status == null) return false;

            status.IsActive = false;
            status.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Statuses.UpdateAsync(status);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
