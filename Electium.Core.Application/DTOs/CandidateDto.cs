using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class CandidateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
        public int PoliticalPartyId { get; set; }
        public string PoliticalPartyAcronym { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
