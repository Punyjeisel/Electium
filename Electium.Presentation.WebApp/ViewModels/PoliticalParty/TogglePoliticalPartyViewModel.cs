namespace Electium.Presentation.WebApp.ViewModels.PoliticalParty
{
    public class TogglePoliticalPartyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool WillActivate { get; set; }

        public string Message => WillActivate
            ? "¿Está seguro que desea activar este partido?"
            : "¿Está seguro que desea desactivar este partido?";
    }
}
