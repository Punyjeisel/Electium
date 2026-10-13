using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    // Saves uploaded files and returns the relative path
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream fileStream, string fileName, string folder);
        void Delete(string relativePath);
    }
}
