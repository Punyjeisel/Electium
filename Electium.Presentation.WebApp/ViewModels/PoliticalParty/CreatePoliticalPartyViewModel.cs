using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.PoliticalParty
{
    public class CreatePoliticalPartyViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre del partido")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Las siglas son obligatorias.")]
        [Display(Name = "Siglas")]
        public string Acronym { get; set; } = string.Empty;

        [Required(ErrorMessage = "El logo es obligatorio.")]
        [Display(Name = "Logo del partido")]
        public IFormFile Logo { get; set; } = null!;
    }
}
