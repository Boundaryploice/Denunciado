using AutoMapper;
using DenunciaDo.Application.DTOs;
using DenunciaDo.Application.Interfaces;
using DenunciaDo.Core.Application.DTOs;
using DenunciaDo.Core.Application.Interfaces;
using DenunciaDo.Core.Domain.Entities;
using DenunciaDo.Core.Domain.Interfaces.Repositories;
using DenunciaDo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenunciaDo.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService,
            IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }

            // Buscar usuario por email
            var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email.Trim().ToLower());

            if (user == null || !user.IsActive)
            {
                return null;
            }

            // Verificar contraseña
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // Actualizar Device ID si se proporcionó
            if (!string.IsNullOrEmpty(request.DeviceId) && user.DeviceId != request.DeviceId)
            {
                user.DeviceId = request.DeviceId;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();
            }

            // Obtener roles del usuario
            var userRoles = await _unitOfWork.UserRoles.GetUserRolesByUserIdAsync(user.Id);
            var rolesList = new List<string>();

            foreach (var userRole in userRoles)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(userRole.RoleId);
                if (role != null && role.IsActive)
                {
                    rolesList.Add(role.Name);
                }
            }

            // Si no tiene roles, asignar rol de usuario por defecto
            if (rolesList.Count == 0)
            {
                var defaultRole = await _unitOfWork.Roles.GetRoleByNameAsync("User");
                if (defaultRole != null)
                {
                    var newUserRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = defaultRole.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _unitOfWork.UserRoles.AddAsync(newUserRole);
                    await _unitOfWork.CompleteAsync();

                    rolesList.Add(defaultRole.Name);
                }
            }

            // Generar token JWT
            var token = await _tokenService.CreateTokenAsync(user, rolesList);

            // Mapear usuario a DTO
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = rolesList;

            return new LoginResponseDto
            {
                Token = token,
                User = userDto
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
        {
            // Validar datos de entrada
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.NickName))
            {
                return null;
            }

            // Normalizar email y nickname
            var normalizedEmail = request.Email.Trim().ToLower();
            var normalizedNickName = request.NickName.Trim().ToLower();

            // Verificar si el email ya existe
            bool emailExists = await _unitOfWork.Users.CheckEmailExistsAsync(normalizedEmail);
            if (emailExists)
            {
                throw new InvalidOperationException("Ya existe un usuario con este email.");
            }

            // Verificar si el nickname ya existe
            bool nicknameExists = await _unitOfWork.Users.CheckNicknameExistsAsync(normalizedNickName);
            if (nicknameExists)
            {
                throw new InvalidOperationException("Ya existe un usuario con este nickname.");
            }

            // Crear entidad usuario
            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = normalizedEmail,
                NickName = request.NickName.Trim(),
                DeviceId = request.DeviceId ?? Guid.NewGuid().ToString(),
                IsAnonymous = false,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Hash de la contraseña
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // Agregar usuario a la base de datos
            await _unitOfWork.Users.AddAsync(user);

            // Crear perfil de usuario si se proporcionaron datos
            if (!string.IsNullOrWhiteSpace(request.Address) ||
                !string.IsNullOrWhiteSpace(request.Phone) ||
                !string.IsNullOrWhiteSpace(request.Identification))
            {
                var profile = new UserProfile
                {
                    UserId = user.Id,
                    Address = request.Address?.Trim(),
                    Phone = request.Phone?.Trim(),
                    Identification = request.Identification?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.UserProfiles.AddAsync(profile);
            }

            // Asignar rol de usuario por defecto
            var userRole = await _unitOfWork.Roles.GetRoleByNameAsync("User");
            if (userRole != null)
            {
                var newUserRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.UserRoles.AddAsync(newUserRole);
            }

            // Guardar cambios
            await _unitOfWork.CompleteAsync();

            // Mapear y retornar usuario DTO
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = new List<string> { "User" };

            return userDto;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return false;
            }

            // Verificar que las contraseñas coincidan
            if (request.NewPassword != request.ConfirmPassword)
            {
                return false;
            }

            // Obtener usuario
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return false;
            }

            // Verificar contraseña actual
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return false;
            }

            // Actualizar contraseña
            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // Métodos adicionales útiles

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            // TODO: Implementar recuperación de contraseña
            // Esta funcionalidad requiere envío de emails
            throw new NotImplementedException("Funcionalidad de recuperación de contraseña pendiente de implementar.");
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            // TODO: Implementar reset de contraseña con token
            throw new NotImplementedException("Funcionalidad de reset de contraseña pendiente de implementar.");
        }

        public async Task<bool> ConfirmEmailAsync(int userId, string token)
        {
            // TODO: Implementar confirmación de email
            throw new NotImplementedException("Funcionalidad de confirmación de email pendiente de implementar.");
        }

        public async Task<UserDto> CreateAnonymousUserAsync(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
            }

            // Verificar si ya existe un usuario anónimo con este device ID
            var existingUsers = await _unitOfWork.Users.GetAsync(u => u.DeviceId == deviceId && u.IsAnonymous);
            if (existingUsers.Any())
            {
                return _mapper.Map<UserDto>(existingUsers.First());
            }

            // Crear usuario anónimo
            var anonymousUser = new User
            {
                FirstName = "Usuario",
                LastName = "Anónimo",
                Email = $"anonymous_{deviceId}@denuncia.do",
                NickName = $"anonymous_{deviceId.Substring(0, 8)}",
                DeviceId = deviceId,
                IsAnonymous = true,
                PasswordHash = _passwordHasher.HashPassword(new User(), Guid.NewGuid().ToString()), // Contraseña aleatoria
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(anonymousUser);

            // Asignar rol de usuario
            var userRole = await _unitOfWork.Roles.GetRoleByNameAsync("User");
            if (userRole != null)
            {
                var newUserRole = new UserRole
                {
                    UserId = anonymousUser.Id,
                    RoleId = userRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.UserRoles.AddAsync(newUserRole);
            }

            await _unitOfWork.CompleteAsync();

            var userDto = _mapper.Map<UserDto>(anonymousUser);
            userDto.Roles = new List<string> { "User" };

            return userDto;
        }

        public async Task<bool> ConvertAnonymousToRegisteredAsync(int anonymousUserId, RegisterRequestDto registerRequest)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(anonymousUserId);
            if (user == null || !user.IsAnonymous)
            {
                return false;
            }

            // Verificar que el email y nickname no existan
            bool emailExists = await _unitOfWork.Users.CheckEmailExistsAsync(registerRequest.Email.ToLower());
            bool nicknameExists = await _unitOfWork.Users.CheckNicknameExistsAsync(registerRequest.NickName.ToLower());

            if (emailExists || nicknameExists)
            {
                return false;
            }

            // Convertir usuario anónimo a registrado
            user.FirstName = registerRequest.FirstName.Trim();
            user.LastName = registerRequest.LastName.Trim();
            user.Email = registerRequest.Email.Trim().ToLower();
            user.NickName = registerRequest.NickName.Trim();
            user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.Password);
            user.IsAnonymous = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);

            // Crear perfil si se proporcionaron datos
            if (!string.IsNullOrWhiteSpace(registerRequest.Address) ||
                !string.IsNullOrWhiteSpace(registerRequest.Phone) ||
                !string.IsNullOrWhiteSpace(registerRequest.Identification))
            {
                var profile = new UserProfile
                {
                    UserId = user.Id,
                    Address = registerRequest.Address?.Trim(),
                    Phone = registerRequest.Phone?.Trim(),
                    Identification = registerRequest.Identification?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.UserProfiles.AddAsync(profile);
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
