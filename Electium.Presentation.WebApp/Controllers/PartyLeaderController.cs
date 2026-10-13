using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Presentation.WebApp.ViewModels.PartyLeader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Electium.Presentation.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PartyLeaderController : Controller
    {
        private readonly IPartyLeaderService _service;

        public PartyLeaderController(IPartyLeaderService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var relations = await _service.GetAllWithIncludesAsync();
            var canModify = await _service.CanModifyAsync();

            var vm = new PartyLeaderIndexViewModel
            {
                Relations = relations,
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
                    "No se puede crear asignaciones mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CreatePartyLeaderViewModel();
            await FillListsAsync(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePartyLeaderViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await FillListsAsync(vm);
                return View(vm);
            }

            var result = await _service.CreateAsync(new CreatePartyLeaderDto
            {
                UserId = vm.UserId,
                PoliticalPartyId = vm.PoliticalPartyId
            });

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                await FillListsAsync(vm);
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
                    "No se puede eliminar asignaciones mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var relations = await _service.GetAllWithIncludesAsync();
            var relation = relations.FirstOrDefault(r => r.Id == id);
            if (relation == null) return NotFound();

            var vm = new DeletePartyLeaderViewModel
            {
                Id = relation.Id,
                UserFullName = relation.UserFullName,
                PartyAcronym = relation.PoliticalPartyAcronym
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

        private async Task FillListsAsync(CreatePartyLeaderViewModel vm)
        {
            var leaders = await _service.GetAvailableLeadersAsync();
            var parties = await _service.GetActivePartiesAsync();

            vm.Leaders = leaders.Select(l => new SelectListItem
            {
                Value = l.UserId.ToString(),
                Text = l.FullName
            }).ToList();

            vm.Parties = parties.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Acronym} - {p.Name}"
            }).ToList();
        }
    }
}
