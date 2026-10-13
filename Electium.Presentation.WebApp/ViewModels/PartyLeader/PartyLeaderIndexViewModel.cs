using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.PartyLeader
{
    public class PartyLeaderIndexViewModel
    {
        public List<PartyLeaderDto> Relations { get; set; } = new();
        public bool CanModify { get; set; }
    }
}
