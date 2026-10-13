using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class CreateCandidateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
        public int PoliticalPartyId { get; set; }
    }
}

