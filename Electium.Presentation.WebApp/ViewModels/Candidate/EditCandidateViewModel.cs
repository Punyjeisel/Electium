using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.Candidate
{
    public class EditCandidateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Foto actual")]
        public string? CurrentPhotoPath { get; set; }

        [Display(Name = "Cambiar foto (opcional)")]
        public IFormFile? Photo { get; set; }
    }
}
