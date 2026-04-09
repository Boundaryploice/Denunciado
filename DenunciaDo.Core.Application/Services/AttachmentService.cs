using AutoMapper;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace DenunciaDo.Application.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage; // Usando interfaz del dominio

        public AttachmentService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
        }

        public async Task<AttachmentDto> GetAttachmentByIdAsync(int id)
        {
            var attachment = await _unitOfWork.Attachments.GetByIdAsync(id);
            return attachment != null ? _mapper.Map<AttachmentDto>(attachment) : null;
        }

        public async Task<List<AttachmentDto>> GetAttachmentsByComplaintAsync(int complaintId)
        {
            var attachments = await _unitOfWork.Attachments.GetAttachmentsByComplaintAsync(complaintId);
            return _mapper.Map<List<AttachmentDto>>(attachments);
        }

        public async Task<List<AttachmentDto>> UploadAttachmentsAsync(int complaintId, IFormFileCollection files)
        {
            var attachments = new List<Attachment>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var filePath = await _fileStorage.SaveFileAsync(stream, file.FileName, "attachments");

                    var attachment = new Attachment
                    {
                        FileName = file.FileName,
                        FilePath = filePath,
                        ContentType = file.ContentType,
                        FileSize = file.Length,
                        ComplaintId = complaintId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _unitOfWork.Attachments.AddAsync(attachment);
                    attachments.Add(attachment);
                }
            }

            await _unitOfWork.CompleteAsync();
            return _mapper.Map<List<AttachmentDto>>(attachments);
        }

        public async Task<bool> DeleteAttachmentAsync(int id)
        {
            var attachment = await _unitOfWork.Attachments.GetByIdAsync(id);
            if (attachment == null) return false;

            // Delete file from storage
            await _fileStorage.DeleteFileAsync(attachment.FilePath, "attachments");

            // Delete from database
            await _unitOfWork.Attachments.DeleteAsync(attachment);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
