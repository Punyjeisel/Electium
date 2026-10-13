using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class AvailablePartyDto
    {
        public int Id { get; set; }
        public string Acronym { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
