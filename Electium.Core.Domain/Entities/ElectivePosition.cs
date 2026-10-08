using System;
using System.Collections.Generic;
using System.Text;
using Electium.Core.Domain.Common;

namespace Electium.Core.Domain.Entities
{
    public class ElectivePosition : BaseEntity
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
