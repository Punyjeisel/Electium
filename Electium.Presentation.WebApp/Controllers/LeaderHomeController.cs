using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Presentation.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "PoliticalLeader")]
    public class LeaderHomeController : Controller
    {
        private readonly IPartyLeaderService _partyLeaderService;
        private readonly ILeaderDashboardService _dashboardService;

        public LeaderHomeController(
            IPartyLeaderService partyLeaderService,
            ILeaderDashboardService dashboardService)
        {
            _partyLeaderService = partyLeaderService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return RedirectToAction("AccessDenied", "Account");

            if (!int.TryParse(claim.Value, out var userId))
                return RedirectToAction("AccessDenied", "Account");

            var partyId = await _partyLeaderService.GetPartyIdByUserIdAsync(userId);
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var dashboard = await _dashboardService.GetDashboardAsync(partyId.Value);

            return View(new LeaderHomeViewModel { Dashboard = dashboard });
        }
    }
}
