using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Presentation.WebApp.ViewModels.Alliance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "PoliticalLeader")]
    public class AllianceController : Controller
    {
        private readonly IAllianceService _service;
        private readonly IPartyLeaderService _partyLeaderService;

        public AllianceController(
            IAllianceService service,
            IPartyLeaderService partyLeaderService)
        {
            _service = service;
            _partyLeaderService = partyLeaderService;
        }

        public async Task<IActionResult> Index()
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var received = await _service.GetReceivedPendingAsync(partyId.Value);
            var sent = await _service.GetSentAsync(partyId.Value);
            var active = await _service.GetActiveAsync(partyId.Value);

            var vm = new AllianceIndexViewModel
            {
                ReceivedPending = received,
                Sent = sent,
                Active = active,
                CanModify = true   // real check done in service actions
            };

            return View(vm);
        }

        // --- SEND NEW REQUEST ---

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var vm = new CreateAllianceRequestViewModel();
            await FillPartiesAsync(vm, partyId.Value);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAllianceRequestViewModel vm)
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            if (!ModelState.IsValid)
            {
                await FillPartiesAsync(vm, partyId.Value);
                return View(vm);
            }

            try
            {
                await _service.CreateRequestAsync(new CreateAllianceRequestDto
                {
                    RequesterPartyId = partyId.Value,
                    ReceiverPartyId = vm.ReceiverPartyId
                });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await FillPartiesAsync(vm, partyId.Value);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // --- ACCEPT ---

        [HttpGet]
        public async Task<IActionResult> Accept(int id)
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var received = await _service.GetReceivedPendingAsync(partyId.Value);
            var request = received.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var vm = new ConfirmAllianceViewModel
            {
                Id = request.Id,
                PartyName = request.RequesterPartyName,
                PartyAcronym = request.RequesterPartyAcronym,
                Action = "aceptar"
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Accept")]
        public async Task<IActionResult> AcceptConfirmed(int id)
        {
            await _service.AcceptRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // --- REJECT ---

        [HttpGet]
        public async Task<IActionResult> Reject(int id)
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var received = await _service.GetReceivedPendingAsync(partyId.Value);
            var request = received.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var vm = new ConfirmAllianceViewModel
            {
                Id = request.Id,
                PartyName = request.RequesterPartyName,
                PartyAcronym = request.RequesterPartyAcronym,
                Action = "rechazar"
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Reject")]
        public async Task<IActionResult> RejectConfirmed(int id)
        {
            await _service.RejectRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // --- DELETE SENT REQUEST ---

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var sent = await _service.GetSentAsync(partyId.Value);
            var request = sent.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var vm = new ConfirmAllianceViewModel
            {
                Id = request.Id,
                PartyName = request.ReceiverPartyName,
                PartyAcronym = request.ReceiverPartyAcronym,
                Action = "eliminar"
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteRequestAsync(id);
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

        private async Task FillPartiesAsync(CreateAllianceRequestViewModel vm, int partyId)
        {
            var parties = await _service.GetPartiesForRequestAsync(partyId);

            vm.Parties = parties.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Acronym} - {p.Name}"
            }).ToList();
        }
    }
}
