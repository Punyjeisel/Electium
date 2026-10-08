using System;
using System.Collections.Generic;
using System.Text;
using Electium.Core.Domain.Common;
using Electium.Core.Domain.Enums;

namespace Electium.Core.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
    }
}
