using Electium.Core.Application.Common;
using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace Electium.Core.Application.Interfaces
{
    public interface IUserService : IGenericService<UserDto, User>
    {
        Task<bool> AnyAsync();
        Task<bool> IsUserNameTakenAsync(string userName, int? excludeUserId = null);
        Task<bool> CanModifyAsync();
        Task<UpdateUserDto?> GetForEditAsync(int id);
        Task<OperationResult> CreateAsync(CreateUserDto dto);
        Task<OperationResult> UpdateWithPasswordAsync(UpdateUserDto dto);
        Task<OperationResult> ToggleActiveAsync(int id);
        Task<UserDto?> ValidateCredentialsAsync(string userName, string password);
    }
}
