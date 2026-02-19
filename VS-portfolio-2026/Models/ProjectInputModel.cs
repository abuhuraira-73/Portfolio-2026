using System.Collections.Generic;

namespace VS_portfolio_2026.Models
{
    public class ProjectInputModel
    {
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public List<string> Tags { get; set; } = new List<string>();
        public int DisplayOrder { get; set; }
    }
}

