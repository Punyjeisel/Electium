using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class AvailableLeaderDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
