using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Domain.Enums
{
    // Result of a login attempt
    public enum LoginResult
    {
        Success = 1,
        InvalidCredentials = 2,
        UserInactive = 3,
        LeaderWithoutParty = 4
    }
}
