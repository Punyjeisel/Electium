using Electium.Core.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
