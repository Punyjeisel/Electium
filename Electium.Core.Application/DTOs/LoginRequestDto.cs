using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class LoginRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
