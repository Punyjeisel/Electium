using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.CandidatePosition
{
    public class CreateCandidatePositionViewModel
    {
        [Required(ErrorMessage = "El candidato es obligatorio.")]
        [Display(Name = "Candidato")]
        public int CandidateId { get; set; }

        [Required(ErrorMessage = "El puesto es obligatorio.")]
        [Display(Name = "Puesto electivo")]
        public int ElectivePositionId { get; set; }

        public List<SelectListItem> Candidates { get; set; } = new();
        public List<SelectListItem> Positions { get; set; } = new();
    }
}
