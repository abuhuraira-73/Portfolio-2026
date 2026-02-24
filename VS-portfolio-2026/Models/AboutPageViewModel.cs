using System.Collections.Generic;

namespace VS_portfolio_2026.Models
{
    public class AboutPageViewModel
    {
        public List<Education> Educations { get; set; } = new();
        public List<Experience> Experiences { get; set; } = new();
    }
}
