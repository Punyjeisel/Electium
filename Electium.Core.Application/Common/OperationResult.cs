using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Common
{
    // Standard result returned by services when the outcome
    // needs to be communicated to the UI
    public class OperationResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }

        public static OperationResult Ok() => new() { Success = true };

        public static OperationResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message };
    }
}
