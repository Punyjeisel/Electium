using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Enums;
using Electium.Presentation.WebApp.ViewModels.Election;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ElectionController : Controller
    {
        private readonly IElectionService _service;

        public ElectionController(IElectionService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var elections = await _service.GetAllOrderedAsync();
            var hasActive = await _service.HasActiveElectionAsync();

            var vm = new ElectionIndexViewModel
            {
                Elections = elections,
                HasActive = hasActive
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (await _service.HasActiveElectionAsync())
            {
                TempData["Error"] = "Ya existe una elección activa en el sistema.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CreateElectionViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateElectionViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.CreateAsync(new CreateElectionDto
            {
                Name = vm.Name,
                Date = vm.Date
            });

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Finish(int id)
        {
            var info = await _service.GetFinishInfoAsync(id);
            if (info == null) return NotFound();

            var vm = new FinishElectionViewModel
            {
                Id = info.Id,
                Name = info.Name
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Finish")]
        public async Task<IActionResult> FinishConfirmed(int id)
        {
            var result = await _service.FinishAsync(id);

            if (!result.Success)
                TempData["Error"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            var election = await _service.GetByIdAsync(id);
            if (election == null) return NotFound();

            if (election.Status != ElectionStatus.Finished)
            {
                TempData["Error"] = "Solo puede ver resultados de elecciones finalizadas.";
                return RedirectToAction(nameof(Index));
            }

            var results = await _service.GetResultsAsync(id);
            if (results == null) return NotFound();

            var vm = new ElectionResultsViewModel
            {
                ElectionId = election.Id,
                ElectionName = election.Name,
                Results = results
            };

            return View(vm);
        }
    }
}
