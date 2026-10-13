using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.Voting
{
    public class CandidateListViewModel
    {
        public int ElectionId { get; set; }
        public int ElectivePositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public List<VotingCandidateDto> Candidates { get; set; } = new();

        public int SelectedCandidateId { get; set; }
        public bool SelectNone { get; set; }
    }
}
