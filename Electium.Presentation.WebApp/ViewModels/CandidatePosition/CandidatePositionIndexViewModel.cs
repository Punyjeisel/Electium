using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.CandidatePosition
{
    public class CandidatePositionIndexViewModel
    {
        public List<CandidatePositionDto> Assignments { get; set; } = new();
        public bool CanModify { get; set; }
    }
}
