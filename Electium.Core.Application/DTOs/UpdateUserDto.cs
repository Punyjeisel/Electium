using System;
using System.Collections.Generic;
using System.Text;
using Electium.Core.Domain.Enums;

namespace Electium.Core.Application.DTOs
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}
