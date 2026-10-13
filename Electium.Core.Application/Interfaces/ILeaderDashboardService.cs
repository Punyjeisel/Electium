using Electium.Core.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface ILeaderDashboardService
    {
        Task<LeaderDashboardDto?> GetDashboardAsync(int partyId);
    }
}
