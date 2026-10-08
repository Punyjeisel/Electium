using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
