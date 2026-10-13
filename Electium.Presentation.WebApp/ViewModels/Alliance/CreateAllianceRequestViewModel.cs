using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.Alliance
{
    public class CreateAllianceRequestViewModel
    {
        [Required(ErrorMessage = "El partido es obligatorio.")]
        [Display(Name = "Partido político")]
        public int ReceiverPartyId { get; set; }

        public List<SelectListItem> Parties { get; set; } = new();
    }
}
