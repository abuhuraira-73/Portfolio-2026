namespace VS_portfolio_2026.Models
{
    public class ExperienceInputModel
    {
        public string Year { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Company { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
