namespace Electium.Presentation.WebApp.ViewModels.Election
{
    public class FinishElectionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Message => "¿Está seguro que desea finalizar esta elección?";
    }
}
