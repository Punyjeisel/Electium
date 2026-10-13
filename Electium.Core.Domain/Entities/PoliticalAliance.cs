using Electium.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;



namespace Electium.Core.Domain.Entities
{
    public class PoliticalAlliance : BaseEntity
    {
        public int PartyAId { get; set; }
        public PoliticalParty PartyA { get; set; }

        public int PartyBId { get; set; }
        public PoliticalParty PartyB { get; set; }

        public DateTime AcceptedDate { get; set; }
    }
}
