using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    // Extracts the identity number from an ID card image
    public interface IOcrService
    {
        Task<string?> ExtractIdentityNumberAsync(Stream imageStream);
    }
}
