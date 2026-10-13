using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class RegisterVoteDto
    {
        public int ElectionId { get; set; }
        public int CitizenId { get; set; }
        public int ElectivePositionId { get; set; }
        public int? CandidateId { get; set; }   // null = "Ninguno"
    }
}
