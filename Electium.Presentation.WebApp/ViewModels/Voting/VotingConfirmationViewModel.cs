using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.Voting
{
    public class VotingConfirmationViewModel
    {
        public int ElectionId { get; set; }
        public List<VotingPositionDto> Positions { get; set; } = new();
        public List<int> CompletedPositionIds { get; set; } = new();

        public bool IsCompleted(int positionId) => CompletedPositionIds.Contains(positionId);
    }
}
