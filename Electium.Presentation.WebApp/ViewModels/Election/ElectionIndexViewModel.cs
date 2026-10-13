using Electium.Core.Application.DTOs;

namespace Electium.Presentation.WebApp.ViewModels.Election
{
    public class ElectionIndexViewModel
    {
        public List<ElectionDto> Elections { get; set; } = new();
        public bool HasActive { get; set; }
    }
}
