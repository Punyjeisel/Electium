namespace Electium.Presentation.WebApp.ViewModels.CandidatePosition
{
    public class DeleteCandidatePositionViewModel
    {
        public int Id { get; set; }
        public string CandidateFullName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;

        public string Message =>
            "¿Está seguro que desea desvincular este candidato de este puesto electivo?";
    }
}