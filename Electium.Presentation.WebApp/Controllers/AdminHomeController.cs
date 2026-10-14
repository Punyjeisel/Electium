using Electium.Core.Application.Interfaces;
using Electium.Presentation.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminHomeController : Controller
    {
        private readonly IElectionService _electionService;

        public AdminHomeController(IElectionService electionService)
        {
            _electionService = electionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await BuildViewModelAsync(null);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int selectedYear)
        {
            var vm = await BuildViewModelAsync(selectedYear);
            return View(vm);
        }

        private async Task<AdminHomeViewModel> BuildViewModelAsync(int? requestedYear)
        {
            var years = await _electionService.GetAvailableYearsAsync();

            var vm = new AdminHomeViewModel
            {
                Years = years.Select(y => new SelectListItem
                {
                    Value = y.ToString(),
                    Text = y.ToString()
                }).ToList()
            };

            if (!years.Any()) return vm;

            vm.SelectedYear = requestedYear ?? years.First();
            vm.Summary = await _electionService.GetSummaryByYearAsync(vm.SelectedYear);

            return vm;
        }
    }
}
