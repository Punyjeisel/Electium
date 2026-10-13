using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.ElectivePosition
{
    public class CreateElectivePositionViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre del puesto")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;
    }
}
