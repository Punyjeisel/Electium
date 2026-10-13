using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.PoliticalParty
{
    public class EditPoliticalPartyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre del partido")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Las siglas son obligatorias.")]
        [Display(Name = "Siglas")]
        public string Acronym { get; set; } = string.Empty;

        [Display(Name = "Logo actual")]
        public string? CurrentLogoPath { get; set; }

        [Display(Name = "Cambiar logo (opcional)")]
        public IFormFile? Logo { get; set; }
    }
}
