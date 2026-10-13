using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.Candidate
{
    public class CandidateIndexViewModel
    {
        public List<CandidateDto> Candidates { get; set; } = new();
        public bool CanModify { get; set; }
    }
}
