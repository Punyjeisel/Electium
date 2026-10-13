using System.ComponentModel.DataAnnotations;

namespace Electium.Presentation.WebApp.ViewModels.Election
{
    public class CreateElectionViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre de la elección")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de realización")]
        public DateTime Date { get; set; } = DateTime.Today;
    }
}
