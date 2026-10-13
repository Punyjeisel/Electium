using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.Citizen
{
    public class CitizenIndexViewModel
    {
        public List<CitizenDto> Citizens { get; set; } = new();
        public bool CanModify { get; set; }
    }
}
