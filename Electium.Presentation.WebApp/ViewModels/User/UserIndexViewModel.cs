using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.User
{
    public class UserIndexViewModel
    {
        public List<UserDto> Users { get; set; } = new();
        public bool CanModify { get; set; }
    }
}
