using Electium.Core.Domain.Common;
using Electium.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Domain.Entities
{
    public class Election : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public ElectionStatus Status { get; set; } = ElectionStatus.InProcess;
    }
}
