using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class CreateAllianceRequestDto
    {
        public int RequesterPartyId { get; set; }
        public int ReceiverPartyId { get; set; }
    }
}
