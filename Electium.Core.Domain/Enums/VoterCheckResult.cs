using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Domain.Enums
{
    // Result of checking if a citizen can vote
    public enum VoterCheckResult
    {
        CanProceed = 1,
        AlreadyVoted = 2,
        NoActiveElection = 3,
        CitizenInactive = 4,
        CitizenNotFound = 5
    }
}
