using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.Citizen
{
    public class CreateCitizenViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [RegularExpression(@"^\d{3}-?\d{7}-?\d$",
            ErrorMessage = "El documento debe tener 11 dígitos (formato 000-0000000-0).")]
        [Display(Name = "Número de documento")]
        public string IdentityNumber { get; set; } = string.Empty;
    }
}
