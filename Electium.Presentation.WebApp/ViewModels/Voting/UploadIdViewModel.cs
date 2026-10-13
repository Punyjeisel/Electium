using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.Voting
{
    public class UploadIdViewModel
    {
        public string IdentityNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe subir una foto de su cédula.")]
        [Display(Name = "Foto de la cédula (solo parte frontal)")]
        public IFormFile CedulaImage { get; set; } = null!;
    }
}
