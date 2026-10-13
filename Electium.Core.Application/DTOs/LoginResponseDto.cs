using Electium.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class LoginResponseDto
    {
        public LoginResult Result { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public UserRole? Role { get; set; }
    }
}
