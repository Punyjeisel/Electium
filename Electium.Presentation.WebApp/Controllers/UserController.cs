using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Presentation.WebApp.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _service.GetAllAsync();
            var canModify = await _service.CanModifyAsync();

            var vm = new UserIndexViewModel
            {
                Users = users,
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
                    "No se puede crear usuarios mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.CreateAsync(new CreateUserDto
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                UserName = vm.UserName,
                Password = vm.Password,
                ConfirmPassword = vm.ConfirmPassword,
                Role = vm.Role
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
                    "No se puede editar usuarios mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _service.GetForEditAsync(id);
            if (dto == null) return NotFound();

            var vm = new EditUserViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                Role = dto.Role
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.UpdateWithPasswordAsync(new UpdateUserDto
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                UserName = vm.UserName,
                Password = vm.Password,
                ConfirmPassword = vm.ConfirmPassword,
                Role = vm.Role
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
            var users = await _service.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            var vm = new ToggleUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                WillActivate = !user.IsActive
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
