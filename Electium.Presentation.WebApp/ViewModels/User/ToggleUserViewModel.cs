namespace Electium.Presentation.WebApp.ViewModels.User
{
    public class ToggleUserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool WillActivate { get; set; }

        public string Message => WillActivate
            ? "¿Está seguro que desea activar este usuario?"
            : "¿Está seguro que desea desactivar este usuario?";
    }
}
