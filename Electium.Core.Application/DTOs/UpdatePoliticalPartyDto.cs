using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class UpdatePoliticalPartyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Acronym { get; set; } = string.Empty;
        public string? LogoPath { get; set; }   // optional during edit
        public bool IsActive { get; set; }
    }
}
