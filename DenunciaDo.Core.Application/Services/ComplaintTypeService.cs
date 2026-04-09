using AutoMapper;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Core.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenunciaDo.Application.Services
{
    public class ComplaintTypeService : IComplaintTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ComplaintTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ComplaintTypeDto>> GetAllComplaintTypesAsync()
        {
            var complaintTypes = await _unitOfWork.ComplaintTypes.GetAsync(ct => ct.IsActive);
            return _mapper.Map<List<ComplaintTypeDto>>(complaintTypes);
        }

        public async Task<ComplaintTypeDto> GetComplaintTypeByIdAsync(int id)
        {
            var complaintType = await _unitOfWork.ComplaintTypes.GetByIdAsync(id);
            return complaintType != null ? _mapper.Map<ComplaintTypeDto>(complaintType) : null;
        }

        public async Task<ComplaintTypeDto> CreateComplaintTypeAsync(ComplaintTypeDto complaintTypeDto)
        {
            var complaintType = _mapper.Map<ComplaintType>(complaintTypeDto);
            complaintType.CreatedAt = DateTime.UtcNow;
            complaintType.IsActive = true;

            await _unitOfWork.ComplaintTypes.AddAsync(complaintType);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ComplaintTypeDto>(complaintType);
        }

        public async Task<ComplaintTypeDto> UpdateComplaintTypeAsync(ComplaintTypeDto complaintTypeDto)
        {
            var complaintType = await _unitOfWork.ComplaintTypes.GetByIdAsync(complaintTypeDto.Id);
            if (complaintType == null) return null;

            _mapper.Map(complaintTypeDto, complaintType);
            complaintType.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ComplaintTypes.UpdateAsync(complaintType);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ComplaintTypeDto>(complaintType);
        }

        public async Task<bool> DeleteComplaintTypeAsync(int id)
        {
            var complaintType = await _unitOfWork.ComplaintTypes.GetByIdAsync(id);
            if (complaintType == null) return false;

            complaintType.IsActive = false;
            complaintType.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ComplaintTypes.UpdateAsync(complaintType);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
