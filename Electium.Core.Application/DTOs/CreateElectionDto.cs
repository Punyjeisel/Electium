using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.DTOs
{
    public class CreateElectionDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
