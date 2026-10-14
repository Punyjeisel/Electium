using Electium.Core.Domain.Enums;

namespace Electium.Presentation.WebApp.Helpers
{
    public static class EnumExtensions
    {
        public static string ToSpanish(this UserRole role) => role switch
        {
            UserRole.Administrator => "Administrador",
            UserRole.PoliticalLeader => "Dirigente político",
            _ => role.ToString()
        };

        public static List<(UserRole Value, string Label)> GetRoleOptions()
        {
            return new List<(UserRole, string)>
        {
            (UserRole.Administrator, "Administrador"),
            (UserRole.PoliticalLeader, "Dirigente político")
        };
        }
    }
}
