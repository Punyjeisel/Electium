using AutoMapper;
using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Core.Domain.Interfaces;

namespace Electium.Core.Application.Services
{
    public class UserService : GenericService<UserDto, User>, IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IGenericRepository<User> repository, IMapper mapper, IPasswordHasher passwordHasher)
            : base(repository, mapper)
        {
            _userRepository = repository;
            _passwordHasher = passwordHasher;
        }
        public async Task<bool> IsUserNameTakenAsync(string userName, int? excludeUserId = null)
        {
            return await _userRepository.AnyAsync(u =>
            u.UserName == userName && 
            (excludeUserId == null || u.Id != excludeUserId));
        }
        

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.Hash(dto.Password);
            user.IsActive = true;

            await _userRepository.AddAsync(user);

            return _mapper.Map<UserDto>(user);
        }


        public async Task UpdateWithPasswordAsync(UpdateUserDto dto)
        {
            var exiting = await _userRepository.GetByIdAsync(dto.Id);
            if (exiting == null) return;

            exiting.FirstName = dto.FirstName;
            exiting.LastName = dto.LastName;
            exiting.Email = dto.Email;
            exiting.UserName = dto.UserName;
            exiting.Role = dto.Role;
            exiting.IsActive = dto.IsActive;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                exiting.PasswordHash = _passwordHasher.Hash(dto.Password);
            }

            await _userRepository.UpdateAsync(exiting);
        }

        public async Task<UserDto?> ValidateCredentialAsync(string userName, string password)
        {
            var users = await _userRepository.GetAllAsync(u => u.UserName == userName && u.IsActive);
            var user = users.FirstOrDefault();

            if (user == null) return null;
            if (!_passwordHasher.Verify(password, user.PasswordHash)) return null;

            return _mapper.Map<UserDto>(user);
        }
    }
}