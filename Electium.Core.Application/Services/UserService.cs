using AutoMapper;
using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Enums;
using Electium.Core.Domain.Interfaces;


namespace Electium.Core.Application.Services
{
    public class UserService : GenericService<UserDto, User>, IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Election> _electionRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IGenericRepository<User> userRepository,
            IGenericRepository<Election> electionRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher)
            : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _electionRepository = electionRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> AnyAsync()
        {
            return await _userRepository.AnyAsync(u => true);
        }

        public async Task<bool> IsUserNameTakenAsync(string userName, int? excludeUserId = null)
        {
            return await _userRepository.AnyAsync(u =>
                u.UserName == userName &&
                (excludeUserId == null || u.Id != excludeUserId));
        }

        public async Task<bool> CanModifyAsync()
        {
            var hasActive = await _electionRepository.AnyAsync(
                e => e.Status == ElectionStatus.InProcess);

            return !hasActive;
        }

        public async Task<UpdateUserDto?> GetForEditAsync(int id)
        {
            var entity = await _userRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<UpdateUserDto>(entity);
        }

        public async Task<OperationResult> CreateAsync(CreateUserDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede crear usuarios mientras exista una elección activa.");

            if (await IsUserNameTakenAsync(dto.UserName))
                return OperationResult.Fail(
                    "Ya existe un usuario con ese nombre de usuario.");

            if (dto.Password != dto.ConfirmPassword)
                return OperationResult.Fail(
                    "La contraseña y su confirmación no coinciden.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.Hash(dto.Password);
            user.IsActive = true;

            await _userRepository.AddAsync(user);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateWithPasswordAsync(UpdateUserDto dto)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede editar usuarios mientras exista una elección activa.");

            if (await IsUserNameTakenAsync(dto.UserName, dto.Id))
                return OperationResult.Fail(
                    "Ya existe otro usuario con ese nombre de usuario.");

            var existing = await _userRepository.GetByIdAsync(dto.Id);
            if (existing == null)
                return OperationResult.Fail("El usuario no existe.");

            // If a password was typed, it must match its confirmation
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                if (dto.Password != dto.ConfirmPassword)
                    return OperationResult.Fail(
                        "La contraseña y su confirmación no coinciden.");

                existing.PasswordHash = _passwordHasher.Hash(dto.Password);
            }

            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.Email = dto.Email;
            existing.UserName = dto.UserName;
            existing.Role = dto.Role;

            await _userRepository.UpdateAsync(existing);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> ToggleActiveAsync(int id)
        {
            if (!await CanModifyAsync())
                return OperationResult.Fail(
                    "No se puede cambiar el estado de un usuario mientras exista una elección activa.");

            var entity = await _userRepository.GetByIdAsync(id);
            if (entity == null)
                return OperationResult.Fail("El usuario no existe.");

            entity.IsActive = !entity.IsActive;
            await _userRepository.UpdateAsync(entity);

            return OperationResult.Ok();
        }

        public async Task<UserDto?> ValidateCredentialsAsync(string userName, string password)
        {
            var users = await _userRepository.GetAllAsync(u => u.UserName == userName);
            var user = users.FirstOrDefault();

            if (user == null) return null;
            if (!_passwordHasher.Verify(password, user.PasswordHash)) return null;

            return _mapper.Map<UserDto>(user);
        }
    }
}