using Electium.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Shared.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;

        public FileStorageService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public async Task<string> SaveAsync(Stream fileStream, string fileName, string folder)
        {
            // Build a unique file name to avoid collisions
            var extension = Path.GetExtension(fileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";

            var folderPath = Path.Combine(_webRootPath, "uploads", folder);
            Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, uniqueName);

            using var output = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(output);

            // Relative path used by the views
            return $"/uploads/{folder}/{uniqueName}";
        }

        public void Delete(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = Path.Combine(_webRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
