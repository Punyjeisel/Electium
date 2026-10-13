namespace Electium.Presentation.WebApp.ViewModels.PartyLeader
{
    public class DeletePartyLeaderViewModel
    {
        public int Id { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string PartyAcronym { get; set; } = string.Empty;

        public string Message =>
            "¿Está seguro que desea desvincular este dirigente político de este partido?";
    }
}
