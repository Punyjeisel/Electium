using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.Election
{
    public class ElectionResultsViewModel
    {
        public int ElectionId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public List<ElectionResultDto> Results { get; set; } = new();
    }
}
