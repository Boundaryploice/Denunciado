using DenunciaDo.Core.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace DenunciaDo.Application.Interfaces
{
    public interface IAttachmentService
    {
        Task<AttachmentDto> GetAttachmentByIdAsync(int id);
        Task<List<AttachmentDto>> GetAttachmentsByComplaintAsync(int complaintId);
        Task<List<AttachmentDto>> UploadAttachmentsAsync(int complaintId, IFormFileCollection files);
        Task<bool> DeleteAttachmentAsync(int id);
    }
}
