using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Presentation.WebApp.ViewModels.CandidatePosition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "PoliticalLeader")]
    public class CandidatePositionController : Controller
    {
        private readonly ICandidatePositionService _service;
        private readonly IPartyLeaderService _partyLeaderService;

        public CandidatePositionController(
            ICandidatePositionService service,
            IPartyLeaderService partyLeaderService)
        {
            _service = service;
            _partyLeaderService = partyLeaderService;
        }

        public async Task<IActionResult> Index()
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var assignments = await _service.GetByPartyAsync(partyId.Value);
            var canModify = await _service.CanModifyAsync();

            var vm = new CandidatePositionIndexViewModel
            {
                Assignments = assignments,
                CanModify = canModify
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!await _service.CanModifyAsync())
            {
                TempData["Error"] =
                    "No se puede asignar candidatos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var vm = new CreateCandidatePositionViewModel();
            await FillListsAsync(vm, partyId.Value);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCandidatePositionViewModel vm)
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            if (!ModelState.IsValid)
            {
                await FillListsAsync(vm, partyId.Value);
                return View(vm);
            }

            var result = await _service.CreateAsync(new CreateCandidatePositionDto
            {
                CandidateId = vm.CandidateId,
                ElectivePositionId = vm.ElectivePositionId,
                PoliticalPartyId = partyId.Value
            });

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                await FillListsAsync(vm, partyId.Value);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.CanModifyAsync())
            {
                TempData["Error"] =
                    "No se puede desvincular candidatos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var assignments = await _service.GetByPartyAsync(partyId.Value);
            var assignment = assignments.FirstOrDefault(a => a.Id == id);
            if (assignment == null) return NotFound();

            var vm = new DeleteCandidatePositionViewModel
            {
                Id = assignment.Id,
                CandidateFullName = assignment.CandidateFullName,
                PositionName = assignment.ElectivePositionName
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result.Success)
                TempData["Error"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }

        // --- private helpers ---

        private async Task<int?> GetCurrentPartyIdAsync()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return null;
            if (!int.TryParse(claim.Value, out var userId)) return null;

            return await _partyLeaderService.GetPartyIdByUserIdAsync(userId);
        }

        private async Task FillListsAsync(CreateCandidatePositionViewModel vm, int partyId)
        {
            var candidates = await _service.GetAvailableCandidatesAsync(partyId);
            var positions = await _service.GetAvailablePositionsAsync(partyId);

            vm.Candidates = candidates.Select(c => new SelectListItem
            {
                Value = c.CandidateId.ToString(),
                Text = c.IsFromAlly
                    ? $"{c.FullName} ({c.PartyAcronym}) [Aliado]"
                    : $"{c.FullName} ({c.PartyAcronym})"
            }).ToList();

            vm.Positions = positions.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Acronym
            }).ToList();
        }
    }
}
