using AutoMapper;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;

namespace DenunciaDo.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _unitOfWork.Roles.GetAsync(r => r.IsActive);
            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            return role != null ? _mapper.Map<RoleDto>(role) : null;
        }

        public async Task<RoleDto> GetRoleByNameAsync(string name)
        {
            var role = await _unitOfWork.Roles.GetRoleByNameAsync(name);
            return role != null ? _mapper.Map<RoleDto>(role) : null;
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            role.NormalizedName = roleDto.Name.ToUpper();
            role.CreatedAt = DateTime.UtcNow;
            role.IsActive = true;

            await _unitOfWork.Roles.AddAsync(role);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> UpdateRoleAsync(RoleDto roleDto)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleDto.Id);
            if (role == null) return null;

            _mapper.Map(roleDto, role);
            role.NormalizedName = roleDto.Name.ToUpper();
            role.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Roles.UpdateAsync(role);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null) return false;

            role.IsActive = false;
            role.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Roles.UpdateAsync(role);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            // Check if user role already exists
            var existingUserRoles = await _unitOfWork.UserRoles.GetUserRolesByUserIdAsync(userId);
            if (existingUserRoles.Any(ur => ur.RoleId == roleId))
            {
                return false; // Role already assigned
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.UserRoles.AddAsync(userRole);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var userRoles = await _unitOfWork.UserRoles.GetUserRolesByUserIdAsync(userId);
            var userRole = userRoles.FirstOrDefault(ur => ur.RoleId == roleId);

            if (userRole == null) return false;

            await _unitOfWork.UserRoles.DeleteAsync(userRole);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
