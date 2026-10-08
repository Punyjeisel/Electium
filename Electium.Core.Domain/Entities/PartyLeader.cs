using Electium.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

// Make the relationship between the User and PoliticalParty entities, where a User can be a leader of a PoliticalParty

namespace Electium.Core.Domain.Entities
{
    public class PartyLeader : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int PoliticalPartyId { get; set; }
        public PoliticalParty PoliticalParty { get; set; } = null!;
    }
}
