using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class VotingCandidateDto
    {
        public int? CandidateId { get; set; }        // null = "Ninguno"
        public string CandidateName { get; set; } = string.Empty;
        public string PartyAcronym { get; set; } = string.Empty;
        public string? PartyLogoPath { get; set; }
        public string? CandidatePhotoPath { get; set; }
    }
}
