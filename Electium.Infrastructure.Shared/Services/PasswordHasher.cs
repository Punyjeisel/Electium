using Electium.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Shared.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            // BCrypt generates a random salt on each call
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
