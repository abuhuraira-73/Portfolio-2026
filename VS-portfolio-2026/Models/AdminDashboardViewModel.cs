using Microsoft.AspNetCore.Http;

namespace VS_portfolio_2026.Models
{
    public class AdminDashboardViewModel
    {
        // For CV Management
        public string? CurrentCvFilename { get; set; }
        public IFormFile? NewCvFile { get; set; }
        public string? CvUploadSuccessMessage { get; set; }

        // We can add properties for other sections like Blogs here later
    }
}
