using System;
using System.Collections.Generic;
using System.Text;
using Electium.Core.Domain.Common;

namespace Electium.Core.Domain.Entities
{
    public class Citizen : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdentifyNumber { get; set; }
    }
}
