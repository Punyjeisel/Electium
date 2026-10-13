namespace Electium.Presentation.WebApp.ViewModels.Citizen
{
    public class ToggleCitizenViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool WillActivate { get; set; }

        public string Message => WillActivate
            ? $"¿Está seguro que desea activar este ciudadano?"
            : $"¿Está seguro que desea desactivar este ciudadano?";
    }
}
