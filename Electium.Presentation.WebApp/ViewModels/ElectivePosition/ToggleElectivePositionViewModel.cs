namespace Electium.Presentation.WebApp.ViewModels.ElectivePosition
{
    public class ToggleElectivePositionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool WillActivate { get; set; }

        public string Message => WillActivate
            ? $"¿Está seguro que desea activar el puesto {Name}?"
            : $"¿Está seguro que desea desactivar el puesto {Name}?";
    }
}
