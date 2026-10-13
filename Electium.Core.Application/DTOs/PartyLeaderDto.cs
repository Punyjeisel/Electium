using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class PartyLeaderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public int PoliticalPartyId { get; set; }
        public string PoliticalPartyAcronym { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
