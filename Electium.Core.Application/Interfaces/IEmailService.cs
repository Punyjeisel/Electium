using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    // Sends emails (implementation lives in Shared)
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
