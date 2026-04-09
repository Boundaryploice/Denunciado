using AutoMapper;
using DenunciaDo.Application.DTOs;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Core.Domain.Interfaces.Services;
using DenunciaDo.Domain.Entities;
using DenunciaDo.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenunciaDo.Application.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly INotificationService _notificationService;

        public ComplaintService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileStorageService fileStorage,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _notificationService = notificationService;
        }

        public async Task<ComplaintDetailDto> GetComplaintByIdAsync(int id, int? userId = null)
        {
            var complaint = await _unitOfWork.Complaints.GetComplaintWithDetailsAsync(id);

            if (complaint == null || !complaint.IsActive)
            {
                return null;
            }

            var result = _mapper.Map<ComplaintDetailDto>(complaint);

            // Enriquecer con datos adicionales
            await EnrichComplaintDto(result, userId);

            // Obtener comentarios con sus respuestas
            var comments = await _unitOfWork.Comments.GetCommentsByComplaintAsync(id);
            var topLevelComments = comments.Where(c => c.ParentCommentId == null && c.IsActive).ToList();

            result.Comments = new List<CommentDto>();
            foreach (var comment in topLevelComments)
            {
                var commentDto = _mapper.Map<CommentDto>(comment);

                // Obtener respuestas
                var replies = comments.Where(c => c.ParentCommentId == comment.Id && c.IsActive).ToList();
                commentDto.Replies = _mapper.Map<List<CommentDto>>(replies);

                result.Comments.Add(commentDto);
            }

            // Obtener historial
            var history = await _unitOfWork.ComplaintHistories.GetHistoryByComplaintAsync(id);
            result.History = _mapper.Map<List<ComplaintHistoryDto>>(history.Where(h => h.IsActive).OrderBy(h => h.CreatedAt));

            return result;
        }

        public async Task<PaginatedListDto<ComplaintDto>> GetPaginatedComplaintsAsync(int pageIndex, int pageSize, int? userId = null)
        {
            var totalCount = await _unitOfWork.Complaints.CountAsync(c => c.IsActive);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var complaints = await _unitOfWork.Complaints.GetAsync(
                c => c.IsActive,
                o => o.OrderByDescending(c => c.CreatedAt),
                "User,ComplaintType,Status,District,District.Municipality",
                true);

            var pagedComplaints = complaints
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PaginatedListDto<ComplaintDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = _mapper.Map<List<ComplaintDto>>(pagedComplaints)
            };

            // Enriquecer DTOs con datos adicionales
            foreach (var complaintDto in result.Items)
            {
                await EnrichComplaintDto(complaintDto, userId);
            }

            return result;
        }

        public async Task<PaginatedListDto<ComplaintDto>> GetComplaintsByTypeAsync(int typeId, int pageIndex, int pageSize, int? userId = null)
        {
            var totalCount = await _unitOfWork.Complaints.CountAsync(c => c.IsActive && c.ComplaintTypeId == typeId);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var complaints = await _unitOfWork.Complaints.GetComplaintsByTypeAsync(typeId);
            var pagedComplaints = complaints
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PaginatedListDto<ComplaintDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = _mapper.Map<List<ComplaintDto>>(pagedComplaints)
            };

            foreach (var complaintDto in result.Items)
            {
                await EnrichComplaintDto(complaintDto, userId);
            }

            return result;
        }

        public async Task<PaginatedListDto<ComplaintDto>> GetComplaintsByStatusAsync(int statusId, int pageIndex, int pageSize, int? userId = null)
        {
            var totalCount = await _unitOfWork.Complaints.CountAsync(c => c.IsActive && c.StatusId == statusId);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var complaints = await _unitOfWork.Complaints.GetComplaintsByStatusAsync(statusId);
            var pagedComplaints = complaints
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PaginatedListDto<ComplaintDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = _mapper.Map<List<ComplaintDto>>(pagedComplaints)
            };

            foreach (var complaintDto in result.Items)
            {
                await EnrichComplaintDto(complaintDto, userId);
            }

            return result;
        }

        public async Task<PaginatedListDto<ComplaintDto>> GetComplaintsByDistrictAsync(int districtId, int pageIndex, int pageSize, int? userId = null)
        {
            var totalCount = await _unitOfWork.Complaints.CountAsync(c => c.IsActive && c.DistrictId == districtId);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var complaints = await _unitOfWork.Complaints.GetComplaintsByDistrictAsync(districtId);
            var pagedComplaints = complaints
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PaginatedListDto<ComplaintDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = _mapper.Map<List<ComplaintDto>>(pagedComplaints)
            };

            foreach (var complaintDto in result.Items)
            {
                await EnrichComplaintDto(complaintDto, userId);
            }

            return result;
        }

        public async Task<PaginatedListDto<ComplaintDto>> GetComplaintsByUserAsync(int userId, int pageIndex, int pageSize)
        {
            var totalCount = await _unitOfWork.Complaints.CountAsync(c => c.IsActive && c.UserId == userId);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var complaints = await _unitOfWork.Complaints.GetComplaintsByUserAsync(userId);
            var pagedComplaints = complaints
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PaginatedListDto<ComplaintDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = _mapper.Map<List<ComplaintDto>>(pagedComplaints)
            };

            foreach (var complaintDto in result.Items)
            {
                await EnrichComplaintDto(complaintDto, userId);
                // El usuario es propietario de estas denuncias
                complaintDto.HasUserVoted = false; // Reset, ya que es su propia denuncia
            }

            return result;
        }

        public async Task<List<ComplaintDto>> GetRecentComplaintsAsync(int count, int? userId = null)
        {
            var complaints = await _unitOfWork.Complaints.GetRecentComplaintsAsync(count);
            var result = _mapper.Map<List<ComplaintDto>>(complaints);

            foreach (var complaintDto in result)
            {
                await EnrichComplaintDto(complaintDto, userId);
            }

            return result;
        }

        public async Task<List<ComplaintDto>> GetTopVotedComplaintsAsync(int count, int? userId = null)
        {
            var complaints = await _unitOfWork.Complaints.GetTopVotedComplaintsAsync(count);
            var result = _mapper.Map<List<ComplaintDto>>(complaints);

            foreach (var complaintDto in result)
            {
                await EnrichComplaintDto(complaintDto, userId);
            }

            return result;
        }

        public async Task<ComplaintDto> CreateComplaintAsync(int userId, CreateComplaintDto createDto)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(createDto.Title) || createDto.Title.Length < 10)
            {
                throw new ArgumentException("El título debe tener al menos 10 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(createDto.Description) || createDto.Description.Length < 20)
            {
                throw new ArgumentException("La descripción debe tener al menos 20 caracteres.");
            }

            // Verificar que el tipo de denuncia existe
            var complaintType = await _unitOfWork.ComplaintTypes.GetByIdAsync(createDto.ComplaintTypeId);
            if (complaintType == null || !complaintType.IsActive)
            {
                throw new ArgumentException("Tipo de denuncia inválido.");
            }

            // Verificar que el distrito existe (si se especificó)
            if (createDto.DistrictId.HasValue)
            {
                var district = await _unitOfWork.Districts.GetByIdAsync(createDto.DistrictId.Value);
                if (district == null || !district.IsActive)
                {
                    throw new ArgumentException("Distrito inválido.");
                }
            }

            // Guardar imagen principal si se proporcionó
            string imagePath = null;
            if (createDto.Image != null && createDto.Image.Length > 0)
            {
                using var stream = createDto.Image.OpenReadStream();
                imagePath = await _fileStorage.SaveFileAsync(stream, createDto.Image.FileName, "complaints");
            }

            // Crear entidad de denuncia
            var complaint = new Complaint
            {
                Title = createDto.Title.Trim(),
                Description = createDto.Description.Trim(),
                Detail = createDto.Detail?.Trim(),
                Address = createDto.Address?.Trim(),
                Latitude = createDto.Latitude,
                Longitude = createDto.Longitude,
                Image = imagePath,
                UserId = userId,
                ComplaintTypeId = createDto.ComplaintTypeId,
                StatusId = 1, // Pendiente
                DistrictId = createDto.DistrictId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Complaints.AddAsync(complaint);

            // Crear registro de historial
            var history = new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                UserId = userId,
                StatusId = 1,
                Comments = "Denuncia creada por el ciudadano",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.ComplaintHistories.AddAsync(history);

            // Guardar archivos adjuntos si se proporcionaron
            if (createDto.Attachments != null && createDto.Attachments.Count > 0)
            {
                foreach (var file in createDto.Attachments)
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
                            ComplaintId = complaint.Id,
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };

                        await _unitOfWork.Attachments.AddAsync(attachment);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();

            // Notificar a administradores
            await NotifyAdministratorsAboutNewComplaint(complaint);

            // Obtener y retornar denuncia completa
            var result = await GetComplaintByIdAsync(complaint.Id, userId);
            return _mapper.Map<ComplaintDto>(result);
        }

        public async Task<ComplaintDto> UpdateComplaintAsync(int userId, UpdateComplaintDto updateDto)
        {
            var complaint = await _unitOfWork.Complaints.GetByIdAsync(updateDto.Id);

            if (complaint == null || !complaint.IsActive)
            {
                return null;
            }

            // Verificar permisos (solo el propietario puede editar si está pendiente)
            if (complaint.UserId != userId)
            {
                // Verificar si es admin o staff
                var user = await _unitOfWork.Users.GetUserWithRolesAsync(userId);
                var userRoles = user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name).ToList();

                if (!userRoles.Contains("Admin") && !userRoles.Contains("Staff"))
                {
                    throw new UnauthorizedAccessException("No tienes permisos para editar esta denuncia.");
                }
            }
            else if (complaint.StatusId != 1) // Solo se puede editar si está pendiente
            {
                throw new InvalidOperationException("Solo se pueden editar denuncias en estado pendiente.");
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(updateDto.Title) || updateDto.Title.Length < 10)
            {
                throw new ArgumentException("El título debe tener al menos 10 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(updateDto.Description) || updateDto.Description.Length < 20)
            {
                throw new ArgumentException("La descripción debe tener al menos 20 caracteres.");
            }

            // Verificar tipo de denuncia
            var complaintType = await _unitOfWork.ComplaintTypes.GetByIdAsync(updateDto.ComplaintTypeId);
            if (complaintType == null || !complaintType.IsActive)
            {
                throw new ArgumentException("Tipo de denuncia inválido.");
            }

            // Actualizar campos
            complaint.Title = updateDto.Title.Trim();
            complaint.Description = updateDto.Description.Trim();
            complaint.Detail = updateDto.Detail?.Trim();
            complaint.Address = updateDto.Address?.Trim();
            complaint.Latitude = updateDto.Latitude;
            complaint.Longitude = updateDto.Longitude;
            complaint.ComplaintTypeId = updateDto.ComplaintTypeId;
            complaint.DistrictId = updateDto.DistrictId;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Complaints.UpdateAsync(complaint);

            // Crear registro de historial
            var history = new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                UserId = userId,
                StatusId = complaint.StatusId,
                Comments = "Denuncia actualizada",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.ComplaintHistories.AddAsync(history);
            await _unitOfWork.CompleteAsync();

            // Retornar denuncia actualizada
            var result = await GetComplaintByIdAsync(complaint.Id, userId);
            return _mapper.Map<ComplaintDto>(result);
        }

        public async Task<bool> DeleteComplaintAsync(int id, int userId)
        {
            var complaint = await _unitOfWork.Complaints.GetByIdAsync(id);

            if (complaint == null || !complaint.IsActive)
            {
                return false;
            }

            // Verificar permisos
            if (complaint.UserId != userId)
            {
                var user = await _unitOfWork.Users.GetUserWithRolesAsync(userId);
                var userRoles = user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name).ToList();

                if (!userRoles.Contains("Admin"))
                {
                    return false;
                }
            }
            else if (complaint.StatusId != 1) // Solo se puede eliminar si está pendiente
            {
                return false;
            }

            // Soft delete
            complaint.IsActive = false;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Complaints.UpdateAsync(complaint);

            // Registro de historial
            var history = new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                UserId = userId,
                StatusId = complaint.StatusId,
                Comments = "Denuncia eliminada",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.ComplaintHistories.AddAsync(history);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<ComplaintDto> UpdateComplaintStatusAsync(int userId, UpdateComplaintStatusDto updateDto)
        {
            var complaint = await _unitOfWork.Complaints.GetByIdAsync(updateDto.ComplaintId);

            if (complaint == null || !complaint.IsActive)
            {
                return null;
            }

            // Verificar permisos (solo admin y staff pueden cambiar estados)
            var user = await _unitOfWork.Users.GetUserWithRolesAsync(userId);
            var userRoles = user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name).ToList();

            if (!userRoles.Contains("Admin") && !userRoles.Contains("Staff"))
            {
                throw new UnauthorizedAccessException("No tienes permisos para cambiar el estado de las denuncias.");
            }

            // Verificar que el estado existe
            var status = await _unitOfWork.Statuses.GetByIdAsync(updateDto.StatusId);
            if (status == null || !status.IsActive)
            {
                throw new ArgumentException("Estado inválido.");
            }

            var oldStatusId = complaint.StatusId;
            complaint.StatusId = updateDto.StatusId;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Complaints.UpdateAsync(complaint);

            // Crear registro de historial
            var history = new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                UserId = userId,
                StatusId = updateDto.StatusId,
                Comments = updateDto.Comments ?? "Cambio de estado",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.ComplaintHistories.AddAsync(history);
            await _unitOfWork.CompleteAsync();

            // Notificar al propietario de la denuncia
            await NotifyUserAboutStatusChange(complaint, oldStatusId, updateDto.StatusId);

            // Retornar denuncia actualizada
            var result = await GetComplaintByIdAsync(complaint.Id, userId);
            return _mapper.Map<ComplaintDto>(result);
        }

        public async Task<PaginatedListDto<ComplaintDto>> SearchComplaintsAsync(string searchTerm, int pageIndex, int pageSize, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetPaginatedComplaintsAsync(pageIndex, pageSize, userId);
            }

            var complaints = await _unitOfWork.Complaints.GetAsync(
                c => c.IsActive && (
                    c.Title.Contains(searchTerm) ||
                    c.Description.Contains(searchTerm) ||
                    c.Detail.Contains(searchTerm) ||
                    c.Address.Contains(searchTerm)
                ),
                o => o.OrderByDescending(c => c.CreatedAt),
                "User,ComplaintType,Status,District,District.Municipality",
                true);

            var totalCount = complaints.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedComplaints = complaints
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PaginatedListDto<ComplaintDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = _mapper.Map<List<ComplaintDto>>(pagedComplaints)
            };

            foreach (var complaintDto in result.Items)
            {
                await EnrichComplaintDto(complaintDto, userId);
            }

            return result;
        }

        // Métodos privados auxiliares

        private async Task EnrichComplaintDto(ComplaintDto complaintDto, int? userId)
        {
            // Obtener conteos de votos
            complaintDto.UpvoteCount = await _unitOfWork.Votes.GetUpvoteCountByComplaintAsync(complaintDto.Id);
            complaintDto.DownvoteCount = await _unitOfWork.Votes.GetDownvoteCountByComplaintAsync(complaintDto.Id);

            // Obtener conteo de comentarios
            var comments = await _unitOfWork.Comments.GetCommentsByComplaintAsync(complaintDto.Id);
            complaintDto.CommentCount = comments.Count(c => c.IsActive);

            // Verificar si el usuario ha votado
            if (userId.HasValue)
            {
                var vote = await _unitOfWork.Votes.GetVoteByUserAndComplaintAsync(userId.Value, complaintDto.Id);
                if (vote != null)
                {
                    complaintDto.HasUserVoted = true;
                    complaintDto.UserVoteType = vote.IsUpvote;
                }
            }

            // Obtener adjuntos
            var attachments = await _unitOfWork.Attachments.GetAttachmentsByComplaintAsync(complaintDto.Id);
            complaintDto.Attachments = _mapper.Map<List<AttachmentDto>>(attachments.Where(a => a.IsActive));
        }

        private async Task NotifyAdministratorsAboutNewComplaint(Complaint complaint)
        {
            try
            {
                var adminUsers = await _unitOfWork.Users.GetUsersByRoleAsync("Admin");
                var staffUsers = await _unitOfWork.Users.GetUsersByRoleAsync("Staff");

                var usersToNotify = adminUsers.Concat(staffUsers).Where(u => u.IsActive);

                foreach (var user in usersToNotify)
                {
                    var notification = new NotificationDto
                    {
                        UserId = user.Id,
                        Title = "Nueva denuncia recibida",
                        Message = $"Se ha recibido una nueva denuncia: '{complaint.Title}' que requiere revisión.",
                        NotificationType = "NewComplaint",
                        RelatedEntityType = "Complaint",
                        RelatedEntityId = complaint.Id
                    };

                    await _notificationService.CreateNotificationAsync(notification);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the complaint creation
                Console.WriteLine($"Error notifying administrators: {ex.Message}");
            }
        }

        private async Task NotifyUserAboutStatusChange(Complaint complaint, int oldStatusId, int newStatusId)
        {
            try
            {
                var oldStatus = await _unitOfWork.Statuses.GetByIdAsync(oldStatusId);
                var newStatus = await _unitOfWork.Statuses.GetByIdAsync(newStatusId);

                if (oldStatus == null || newStatus == null)
                {
                    return;
                }

                var notification = new NotificationDto
                {
                    UserId = complaint.UserId,
                    Title = "Estado de denuncia actualizado",
                    Message = $"El estado de tu denuncia '{complaint.Title}' ha cambiado de '{oldStatus.Name}' a '{newStatus.Name}'.",
                    NotificationType = "StatusUpdate",
                    RelatedEntityType = "Complaint",
                    RelatedEntityId = complaint.Id
                };

                await _notificationService.CreateNotificationAsync(notification);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the status update
                Console.WriteLine($"Error notifying user about status change: {ex.Message}");
            }
        }
    }
}
