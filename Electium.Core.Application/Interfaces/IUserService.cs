using Electium.Core.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;


namespace Electium.Core.Application.Interfaces
{
    public interface IUserService : IGenericService<UserDto, Electium.Core.Domain.Entities.User>
    { 
        Task<bool> IsUserNameTakenAsync(string userName, int? excludeUserId = null);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task UpdateWithPasswordAsync(UpdateUserDto dto);
        Task<UserDto?> ValidateCredentialAsync(string userName, string password);
    }
}
