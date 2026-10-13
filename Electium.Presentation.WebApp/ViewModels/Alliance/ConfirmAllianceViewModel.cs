namespace Electium.Presentation.WebApp.ViewModels.Alliance
{
    public class ConfirmAllianceViewModel
    {
        public int Id { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string PartyAcronym { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;   // "aceptar", "rechazar", "eliminar"

        public string Message => Action switch
        {
            "aceptar" =>
                $"¿Está seguro que desea aceptar la alianza con el partido {PartyName} {PartyAcronym}?",
            "rechazar" =>
                $"¿Está seguro que desea rechazar la alianza con el partido {PartyName} {PartyAcronym}?",
            "eliminar" =>
                $"¿Está seguro que desea eliminar la solicitud de alianza con el partido {PartyName} {PartyAcronym}?",
            _ => string.Empty
        };

        public string ButtonLabel => Action == "eliminar" ? "Aceptar" : "Aceptar";
        public string ButtonClass => Action == "eliminar" ? "btn-danger" : "btn-primary";
    }
}
