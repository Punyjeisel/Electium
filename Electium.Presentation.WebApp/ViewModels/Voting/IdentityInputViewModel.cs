using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.Voting
{
    public class IdentityInputViewModel
    {
        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [Display(Name = "Número de documento")]
        public string IdentityNumber { get; set; } = string.Empty;
    }
}
