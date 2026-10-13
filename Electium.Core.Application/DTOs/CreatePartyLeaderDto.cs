using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class CreatePartyLeaderDto
    {
        public int UserId { get; set; }
        public int PoliticalPartyId { get; set; }
    }
}
