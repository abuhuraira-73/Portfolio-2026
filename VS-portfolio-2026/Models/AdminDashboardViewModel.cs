using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VS_portfolio_2026.Models
{
    public class AdminDashboardViewModel
    {
        // For CV Management
        public string? CurrentCvFilename { get; set; }
        public IFormFile? NewCvFile { get; set; }
        public string? CvUploadSuccessMessage { get; set; }

        // For Education Management
        public Education NewEducation { get; set; }
        public List<Education> Educations { get; set; }

        // For Experience Management
        public ExperienceInputModel NewExperience { get; set; }
        public List<Experience> Experiences { get; set; }

        // For Blog Management
        public BlogPostInputModel NewBlogPost { get; set; }
        public List<BlogPost> BlogPosts { get; set; }

        // For Contact Management
        public List<Contact> Contacts { get; set; }
    }
}
