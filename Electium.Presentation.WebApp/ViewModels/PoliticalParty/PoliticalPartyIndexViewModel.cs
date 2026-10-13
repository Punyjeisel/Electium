using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.PoliticalParty
{
    public class PoliticalPartyIndexViewModel
    {
        public List<PoliticalPartyDto> Parties { get; set; } = new();
        public bool CanModify { get; set; }
    }
}
