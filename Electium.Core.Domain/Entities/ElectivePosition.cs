using System;
using System.Collections.Generic;
using System.Text;
using Electium.Core.Domain.Common;

namespace Electium.Core.Domain.Entities
{
    public class ElectivePosition : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
