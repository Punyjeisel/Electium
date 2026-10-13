namespace Electium.Presentation.WebApp.ViewModels.Candidate
{
    public class ToggleCandidateViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool WillActivate { get; set; }

        public string Message => WillActivate
            ? "¿Está seguro que desea activar este candidato?"
            : "¿Está seguro que desea desactivar este candidato?";
    }
}
