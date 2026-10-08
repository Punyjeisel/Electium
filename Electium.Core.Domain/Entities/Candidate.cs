using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using Electium.Core.Domain.Common;

namespace Electium.Core.Domain.Entities
{
    public class Candidate : BaseEntity
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoPath { get; set; }
        public int PoliticalPartyId { get; set; }
        public PoliticalParty PoliticalParty { get; set; } = null!;
    }
}
