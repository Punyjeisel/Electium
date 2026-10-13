using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Presentation.WebApp.ViewModels.PoliticalParty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PoliticalPartyController : Controller
    {
        private readonly IPoliticalPartyService _service;
        private readonly IFileStorageService _fileStorage;

        public PoliticalPartyController(
            IPoliticalPartyService service,
            IFileStorageService fileStorage)
        {
            _service = service;
            _fileStorage = fileStorage;
        }

        public async Task<IActionResult> Index()
        {
            var parties = await _service.GetAllAsync();
            var canModify = await _service.CanModifyAsync();

            var vm = new PoliticalPartyIndexViewModel
            {
                Parties = parties,
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
                    "No se puede crear partidos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CreatePoliticalPartyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePoliticalPartyViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Save file first (web concern)
            var logoPath = await _fileStorage.SaveAsync(
                vm.Logo.OpenReadStream(), vm.Logo.FileName, "parties");

            var result = await _service.CreateAsync(new CreatePoliticalPartyDto
            {
                Name = vm.Name,
                Description = vm.Description,
                Acronym = vm.Acronym,
                LogoPath = logoPath
            });

            if (!result.Success)
            {
                // Rollback file if business rule failed
                _fileStorage.Delete(logoPath);
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
                    "No se puede editar partidos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _service.GetForEditAsync(id);
            if (dto == null) return NotFound();

            var vm = new EditPoliticalPartyViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Acronym = dto.Acronym,
                CurrentLogoPath = dto.LogoPath
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPoliticalPartyViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // If a new logo was uploaded, save it (optional)
            string? newLogoPath = null;
            if (vm.Logo != null)
            {
                newLogoPath = await _fileStorage.SaveAsync(
                    vm.Logo.OpenReadStream(), vm.Logo.FileName, "parties");
            }

            var result = await _service.UpdateAsync(new UpdatePoliticalPartyDto
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                Acronym = vm.Acronym,
                LogoPath = newLogoPath
            });

            if (!result.Success)
            {
                if (newLogoPath != null) _fileStorage.Delete(newLogoPath);
                ModelState.AddModelError("", result.ErrorMessage!);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Toggle(int id)
        {
            var parties = await _service.GetAllAsync();
            var party = parties.FirstOrDefault(p => p.Id == id);
            if (party == null) return NotFound();

            var vm = new TogglePoliticalPartyViewModel
            {
                Id = party.Id,
                Name = party.Name,
                WillActivate = !party.IsActive
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
    }
}
