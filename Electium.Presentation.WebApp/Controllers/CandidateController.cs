using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Entities;
using Electium.Presentation.WebApp.ViewModels.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "PoliticalLeader")]
    public class CandidateController : Controller
    {
        private readonly ICandidateService _service;
        private readonly IPartyLeaderService _partyLeaderService;
        private readonly IFileStorageService _fileStorage;

        public CandidateController(
            ICandidateService service,
            IPartyLeaderService partyLeaderService,
            IFileStorageService fileStorage)
        {
            _service = service;
            _partyLeaderService = partyLeaderService;
            _fileStorage = fileStorage;
        }

        public async Task<IActionResult> Index()
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var candidates = await _service.GetByPartyAsync(partyId.Value);
            var canModify = await _service.CanModifyAsync();

            var vm = new CandidateIndexViewModel
            {
                Candidates = candidates,
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
                    "No se puede crear candidatos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CreateCandidateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCandidateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var photoPath = await _fileStorage.SaveAsync(
                vm.Photo.OpenReadStream(), vm.Photo.FileName, "candidates");

            var result = await _service.CreateAsync(new CreateCandidateDto
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                PhotoPath = photoPath,
                PoliticalPartyId = partyId.Value
            });

            if (!result.Success)
            {
                _fileStorage.Delete(photoPath);
                ModelState.AddModelError("", result.ErrorMessage!);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await _service.CanModifyAsync())
            {
                TempData["Error"] =
                    "No se puede editar candidatos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _service.GetForEditAsync(id);
            if (dto == null) return NotFound();

            var vm = new EditCandidateViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CurrentPhotoPath = dto.PhotoPath
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCandidateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            string? newPhotoPath = null;
            if (vm.Photo != null)
            {
                newPhotoPath = await _fileStorage.SaveAsync(
                    vm.Photo.OpenReadStream(), vm.Photo.FileName, "candidates");
            }

            var result = await _service.UpdateAsync(new UpdateCandidateDto
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                PhotoPath = newPhotoPath
            });

            if (!result.Success)
            {
                if (newPhotoPath != null) _fileStorage.Delete(newPhotoPath);
                ModelState.AddModelError("", result.ErrorMessage!);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Toggle(int id)
        {
            var partyId = await GetCurrentPartyIdAsync();
            if (partyId == null) return RedirectToAction("AccessDenied", "Account");

            var candidates = await _service.GetByPartyAsync(partyId.Value);
            var candidate = candidates.FirstOrDefault(c => c.Id == id);
            if (candidate == null) return NotFound();

            var vm = new ToggleCandidateViewModel
            {
                Id = candidate.Id,
                FullName = $"{candidate.FirstName} {candidate.LastName}",
                WillActivate = !candidate.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Toggle")]
        public async Task<IActionResult> ToggleConfirmed(int id)
        {
            var result = await _service.ToggleActiveAsync(id);

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
    }
}
