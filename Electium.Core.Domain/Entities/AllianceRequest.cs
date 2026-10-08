using Electium.Core.Domain.Common;
using Electium.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Domain.Entities
{
    public class AllianceRequest : BaseEntity
    {
        public int RequesterPartyId { get; set; }
        public PoliticalParty RequesterParty { get; set; }

        public int ReceiverPartyId { get; set; }
        public PoliticalParty ReceiverParty { get; set; }

        public DateTime RequestDate { get; set; }
        public AllianceStatus Status { get; set; } = AllianceStatus.Pending;
    }
}
