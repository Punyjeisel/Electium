using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class PoliticalAllianceDto
    {
        public int Id { get; set; }
        public int PartyAId { get; set; }
        public int PartyBId { get; set; }
        public string AllyPartyName { get; set; } = string.Empty;
        public string AllyPartyAcronym { get; set; } = string.Empty;
        public DateTime AcceptedDate { get; set; }
    }
}
