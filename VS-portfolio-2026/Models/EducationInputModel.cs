namespace VS_portfolio_2026.Models
{
    // This model is used specifically for the form binding to avoid issues with the required 'Id' field.
    public class EducationInputModel
    {
        public string Year { get; set; } = null!;
        public string Course { get; set; } = null!;
        public string College { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
