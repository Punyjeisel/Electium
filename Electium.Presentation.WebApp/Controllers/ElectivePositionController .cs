using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Presentation.WebApp.ViewModels.ElectivePosition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ElectivePositionController : Controller
    {
        private readonly IElectivePositionService _service;

        public ElectivePositionController(IElectivePositionService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var positions = await _service.GetAllAsync();
            var canModify = await _service.CanModifyAsync();

            var vm = new ElectivePositionIndexViewModel
            {
                Positions = positions,
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
                    "No se puede crear puestos electivos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CreateElectivePositionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateElectivePositionViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.CreateAsync(new CreateElectivePositionDto
            {
                Name = vm.Name,
                Description = vm.Description
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
                    "No se puede editar puestos electivos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _service.GetForEditAsync(id);
            if (dto == null) return NotFound();

            var vm = new EditElectivePositionViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditElectivePositionViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.UpdateAsync(new UpdateElectivePositionDto
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description
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
            var positions = await _service.GetAllAsync();
            var position = positions.FirstOrDefault(p => p.Id == id);
            if (position == null) return NotFound();

            var vm = new ToggleElectivePositionViewModel
            {
                Id = position.Id,
                Name = position.Name,
                WillActivate = !position.IsActive
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
