using AutoMapper;
using DenunciaDo.Application.DTOs;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Core.Domain.Interfaces.Services;

namespace DenunciaDo.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);

            // Get user roles
            var userRoles = await _unitOfWork.UserRoles.GetUserRolesByUserIdAsync(id);
            userDto.Roles = userRoles.Select(ur => ur.Role.Name).ToList();

            return userDto;
        }

        public async Task<UserDto> GetUserWithProfileAsync(int id)
        {
            var user = await _unitOfWork.Users.GetUserWithProfileAsync(id);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);

            // Get user roles
            var userRoles = await _unitOfWork.UserRoles.GetUserRolesByUserIdAsync(id);
            userDto.Roles = userRoles.Select(ur => ur.Role.Name).ToList();

            return userDto;
        }

        public async Task<PaginatedListDto<UserDto>> GetPaginatedUsersAsync(int pageIndex, int pageSize)
        {
            var totalCount = await _unitOfWork.Users.CountAsync(u => u.IsActive);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var users = await _unitOfWork.Users.GetAsync(
                u => u.IsActive,
                o => o.OrderBy(u => u.FirstName),
                includeString: "UserRoles.Role",
                disableTracking: true);

            var pagedUsers = users
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userDtos = _mapper.Map<List<UserDto>>(pagedUsers);

            // Add roles to each user
            foreach (var userDto in userDtos)
            {
                var user = pagedUsers.First(u => u.Id == userDto.Id);
                userDto.Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            }

            return new PaginatedListDto<UserDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = userDtos
            };
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _unitOfWork.Users.GetUsersByRoleAsync(role);
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateProfileDto updateDto)
        {
            var user = await _unitOfWork.Users.GetUserWithProfileAsync(id);
            if (user == null) return null;

            // Update user basic info
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.NickName = updateDto.NickName;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);

            // Update or create profile
            if (user.Profile == null)
            {
                user.Profile = new UserProfile
                {
                    UserId = user.Id,
                    Address = updateDto.Address,
                    Phone = updateDto.Phone,
                    Bio = updateDto.Bio,
                    BirthDate = updateDto.BirthDate,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await _unitOfWork.UserProfiles.AddAsync(user.Profile);
            }
            else
            {
                user.Profile.Address = updateDto.Address;
                user.Profile.Phone = updateDto.Phone;
                user.Profile.Bio = updateDto.Bio;
                user.Profile.BirthDate = updateDto.BirthDate;
                user.Profile.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.UserProfiles.UpdateAsync(user.Profile);
            }

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWithProfileAsync(userId);
            return user?.Profile != null ? _mapper.Map<UserProfileDto>(user.Profile) : null;
        }

        public async Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateProfileDto updateDto)
        {
            var user = await _unitOfWork.Users.GetUserWithProfileAsync(userId);
            if (user == null) return null;

            if (user.Profile == null)
            {
                user.Profile = new UserProfile
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await _unitOfWork.UserProfiles.AddAsync(user.Profile);
            }

            user.Profile.Address = updateDto.Address;
            user.Profile.Phone = updateDto.Phone;
            user.Profile.Bio = updateDto.Bio;
            user.Profile.BirthDate = updateDto.BirthDate;
            user.Profile.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.UserProfiles.UpdateAsync(user.Profile);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserProfileDto>(user.Profile);
        }
    }
}
