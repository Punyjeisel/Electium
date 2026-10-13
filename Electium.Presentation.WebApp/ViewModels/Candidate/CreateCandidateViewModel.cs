using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.Candidate
{
    public class CreateCandidateViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La foto es obligatoria.")]
        [Display(Name = "Foto del candidato")]
        public IFormFile Photo { get; set; } = null!;
    }
}
