using Electium.Core.Application.Interfaces;
using Tesseract;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Shared.Services
{
    public class OcrService : IOcrService
    {
        private readonly string _tessdataPath;

        public OcrService(string tessdataPath)
        {
            _tessdataPath = tessdataPath;
        }

        public Task<string?> ExtractIdentityNumberAsync(Stream imageStream)
        {
            // Save stream to a temp file because Tesseract needs a path
            var tempFile = Path.GetTempFileName();
            try
            {
                using (var fs = new FileStream(tempFile, FileMode.Create))
                {
                    imageStream.CopyTo(fs);
                }

                using var engine = new TesseractEngine(_tessdataPath, "spa", EngineMode.Default);
                using var img = Pix.LoadFromFile(tempFile);
                using var page = engine.Process(img);

                var text = page.GetText();
                var number = ExtractNumber(text);

                return Task.FromResult(number);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }

        // Find a pattern like 000-0000000-0 or 00000000000
        private static string? ExtractNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            // Remove line breaks and extra spaces
            var cleaned = text.Replace("\n", " ").Replace("\r", " ");

            // Regex: 11 digits with optional dashes
            var match = System.Text.RegularExpressions.Regex.Match(
                cleaned, @"\d{3}-?\d{7}-?\d");

            return match.Success ? match.Value : null;
        }
    }
}
