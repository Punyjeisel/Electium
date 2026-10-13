using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.Alliance
{
    public class AllianceIndexViewModel
    {
        public List<AllianceRequestDto> ReceivedPending { get; set; } = new();
        public List<AllianceRequestDto> Sent { get; set; } = new();
        public List<PoliticalAllianceDto> Active { get; set; } = new();
        public bool CanModify { get; set; }
    }
}
