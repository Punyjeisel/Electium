using Electium.Core.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Electium.Presentation.WebApp.ViewModels
{
    public class AdminHomeViewModel
    {
        public int SelectedYear { get; set; }
        public List<SelectListItem> Years { get; set; } = new();
        public List<ElectionSummaryDto>? Summary { get; set; }
    }
}
