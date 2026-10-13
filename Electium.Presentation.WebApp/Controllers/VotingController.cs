using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Enums;
using Electium.Presentation.WebApp.Services;
using Electium.Presentation.WebApp.ViewModels.Voting;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;

namespace Electium.Presentation.WebApp.Controllers
{
    public class VotingController : Controller
    {
        private readonly IVotingService _votingService;
        private readonly ICitizenService _citizenService;
        private readonly IOcrService _ocrService;

        public VotingController(
            IVotingService votingService,
            ICitizenService citizenService,
            IOcrService ocrService)
        {
            _votingService = votingService;
            _citizenService = citizenService;
            _ocrService = ocrService;
        }

        // --- Step 1: identity number ---

        [HttpGet]
        public IActionResult Index()
        {
            VotingSession.Clear(HttpContext.Session);
            return View(new IdentityInputViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IdentityInputViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var check = await _votingService.CheckVoterAsync(vm.IdentityNumber);

            switch (check.Result)
            {
                case VoterCheckResult.AlreadyVoted:
                    ModelState.AddModelError("", "Ya ha ejercido su derecho al voto.");
                    return View(vm);

                case VoterCheckResult.NoActiveElection:
                    ModelState.AddModelError("", "No hay ningún proceso electoral en estos momentos.");
                    return View(vm);

                case VoterCheckResult.CitizenInactive:
                    ModelState.AddModelError("", "El ciudadano está inactivo.");
                    return View(vm);

                case VoterCheckResult.CitizenNotFound:
                    ModelState.AddModelError("", "El ciudadano no existe en el sistema.");
                    return View(vm);
            }

            // Can proceed → go to ID upload
            TempData["IdentityNumber"] = vm.IdentityNumber;
            TempData["ElectionId"] = check.ActiveElectionId;
            TempData["CitizenId"] = check.CitizenId;

            return RedirectToAction(nameof(Verify));
        }

        // --- Step 2: upload ID card ---

        [HttpGet]
        public IActionResult Verify()
        {
            if (TempData["IdentityNumber"] == null)
                return RedirectToAction(nameof(Index));

            TempData.Keep();
            return View(new UploadIdViewModel
            {
                IdentityNumber = TempData["IdentityNumber"]!.ToString()!
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(UploadIdViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var extracted = await _ocrService.ExtractIdentityNumberAsync(
                vm.CedulaImage.OpenReadStream());

            if (string.IsNullOrWhiteSpace(extracted))
            {
                ModelState.AddModelError("",
                    "No se pudo leer el número de cédula en la imagen. " +
                    "Asegúrese de que la foto sea clara y bien iluminada.");
                return View(vm);
            }

            var match = await _votingService.VerifyIdentityAsync(
                vm.IdentityNumber, extracted);

            if (!match)
            {
                ModelState.AddModelError("",
                    "Los datos extraídos de la foto no coinciden con los datos " +
                    "previamente ingresados por el elector.");
                return View(vm);
            }

            // Success → start voting session
            var electionId = (int)TempData["ElectionId"]!;
            var citizenId = (int)TempData["CitizenId"]!;

            VotingSession.StartSession(HttpContext.Session, electionId, citizenId);

            return RedirectToAction(nameof(Positions));
        }

        // --- Step 3: choose a position ---

        [HttpGet]
        public async Task<IActionResult> Positions()
        {
            var electionId = VotingSession.GetElectionId(HttpContext.Session);
            if (electionId == null || !VotingSession.IsVerified(HttpContext.Session))
                return RedirectToAction(nameof(Index));

            var positions = await _votingService.GetPositionsAsync(electionId.Value);
            var completed = VotingSession.GetCompletedPositionIds(HttpContext.Session);

            var vm = new PositionListViewModel
            {
                ElectionId = electionId.Value,
                Positions = positions,
                CompletedPositionIds = completed
            };

            return View(vm);
        }

        // --- Step 4: choose a candidate ---

        [HttpGet]
        public async Task<IActionResult> Candidates(int positionId)
        {
            var electionId = VotingSession.GetElectionId(HttpContext.Session);
            if (electionId == null || !VotingSession.IsVerified(HttpContext.Session))
                return RedirectToAction(nameof(Index));

            var positions = await _votingService.GetPositionsAsync(electionId.Value);
            var position = positions.FirstOrDefault(p => p.ElectivePositionId == positionId);
            if (position == null) return NotFound();

            var candidates = await _votingService.GetCandidatesByPositionAsync(
                electionId.Value, positionId);

            var vm = new CandidateListViewModel
            {
                ElectionId = electionId.Value,
                ElectivePositionId = positionId,
                PositionName = position.PositionName,
                Candidates = candidates
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Candidates(CandidateListViewModel vm)
        {
            var electionId = VotingSession.GetElectionId(HttpContext.Session);
            if (electionId == null || !VotingSession.IsVerified(HttpContext.Session))
                return RedirectToAction(nameof(Index));

            int? selectedCandidate = vm.SelectNone ? null : vm.SelectedCandidateId;

            if (!vm.SelectNone && vm.SelectedCandidateId == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un candidato antes de votar.");

                var candidates = await _votingService.GetCandidatesByPositionAsync(
                    electionId.Value, vm.ElectivePositionId);
                vm.Candidates = candidates;
                return View(vm);
            }

            VotingSession.AddVote(HttpContext.Session, vm.ElectivePositionId, selectedCandidate);

            return RedirectToAction(nameof(Positions));
        }

        // --- Step 5: confirmation and complete ---

        [HttpGet]
        public async Task<IActionResult> Confirmation()
        {
            var electionId = VotingSession.GetElectionId(HttpContext.Session);
            if (electionId == null || !VotingSession.IsVerified(HttpContext.Session))
                return RedirectToAction(nameof(Index));

            var positions = await _votingService.GetPositionsAsync(electionId.Value);
            var completed = VotingSession.GetCompletedPositionIds(HttpContext.Session);

            var vm = new VotingConfirmationViewModel
            {
                ElectionId = electionId.Value,
                Positions = positions,
                CompletedPositionIds = completed
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete()
        {
            var electionId = VotingSession.GetElectionId(HttpContext.Session);
            var citizenId = VotingSession.GetCitizenId(HttpContext.Session);
            if (electionId == null || citizenId == null)
                return RedirectToAction(nameof(Index));

            var positions = await _votingService.GetPositionsAsync(electionId.Value);
            var votes = VotingSession.GetVotes(HttpContext.Session);

            // Verify all positions have a vote
            var pending = positions
                .Where(p => !votes.ContainsKey(p.ElectivePositionId))
                .Select(p => p.PositionName)
                .ToList();

            if (pending.Any())
            {
                TempData["Error"] =
                    $"Aún falta seleccionar candidato para: {string.Join(", ", pending)}.";
                return RedirectToAction(nameof(Positions));
            }

            // Write each vote (anonymous)
            foreach (var vote in votes)
            {
                await _votingService.RegisterVoteAsync(new RegisterVoteDto
                {
                    ElectionId = electionId.Value,
                    CitizenId = citizenId.Value,
                    ElectivePositionId = vote.Key,
                    CandidateId = vote.Value
                });
            }

            // Mark as voter and clear session
            await _votingService.CompleteVotingAsync(electionId.Value, citizenId.Value);
            VotingSession.Clear(HttpContext.Session);

            TempData["Success"] = "Su voto ha sido registrado. Recibirá un correo con el resumen.";
            return RedirectToAction("Index", "Home");
        }
    }
}
