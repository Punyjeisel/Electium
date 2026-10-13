using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.ElectivePosition
{
    public class ElectivePositionIndexViewModel
    {
        public List<ElectivePositionDto> Positions { get; set; } = new();
        public bool CanModify { get; set; }
    }
}
