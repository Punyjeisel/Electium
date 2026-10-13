using Electium.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class AllianceRequestDto
    {
        public int Id { get; set; }
        public int RequesterPartyId { get; set; }
        public string RequesterPartyName { get; set; } = string.Empty;
        public string RequesterPartyAcronym { get; set; } = string.Empty;

        public int ReceiverPartyId { get; set; }
        public string ReceiverPartyName { get; set; } = string.Empty;
        public string ReceiverPartyAcronym { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; }
        public AllianceStatus Status { get; set; }
    }
}
