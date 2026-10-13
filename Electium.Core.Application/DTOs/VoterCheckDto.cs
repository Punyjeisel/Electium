using Electium.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class VoterCheckDto
    {
        public VoterCheckResult Result { get; set; }
        public int? CitizenId { get; set; }
        public int? ActiveElectionId { get; set; }
    }
}
