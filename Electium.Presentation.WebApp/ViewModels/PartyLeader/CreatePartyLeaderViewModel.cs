using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.PartyLeader
{
    public class CreatePartyLeaderViewModel
    {
        [Required(ErrorMessage = "El dirigente es obligatorio.")]
        [Display(Name = "Dirigente político")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El partido es obligatorio.")]
        [Display(Name = "Partido político")]
        public int PoliticalPartyId { get; set; }

        public List<SelectListItem> Leaders { get; set; } = new();
        public List<SelectListItem> Parties { get; set; } = new();
    }
}
