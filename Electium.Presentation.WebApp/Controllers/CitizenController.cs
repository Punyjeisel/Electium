using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Presentation.WebApp.ViewModels.Citizen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CitizenController : Controller
    {
        private readonly ICitizenService _service;

        public CitizenController(ICitizenService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var citizens = await _service.GetAllAsync();
            var canModify = await _service.CanModifyAsync();

            var vm = new CitizenIndexViewModel
            {
                Citizens = citizens,
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
                    "No se puede crear ciudadanos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CreateCitizenViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCitizenViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.CreateAsync(new CreateCitizenDto
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                IdentityNumber = vm.IdentityNumber
            });

            if (!result.Success)
            {
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
                    "No se puede editar ciudadanos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _service.GetForEditAsync(id);
            if (dto == null) return NotFound();

            var vm = new EditCitizenViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                IdentityNumber = dto.IdentityNumber
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCitizenViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.UpdateAsync(new UpdateCitizenDto
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                IdentityNumber = vm.IdentityNumber
            });

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Toggle(int id)
        {
            var citizens = await _service.GetAllAsync();
            var citizen = citizens.FirstOrDefault(c => c.Id == id);
            if (citizen == null) return NotFound();

            var vm = new ToggleCitizenViewModel
            {
                Id = citizen.Id,
                FullName = $"{citizen.FirstName} {citizen.LastName}",
                WillActivate = !citizen.IsActive
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
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
