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
        public Education NewEducation { get; set; } = default!;
        public List<Education> Educations { get; set; } = new();

        // For Experience Management
        public ExperienceInputModel NewExperience { get; set; } = default!;
        public List<Experience> Experiences { get; set; } = new();

        // For Blog Management
        public BlogPostInputModel NewBlogPost { get; set; } = default!;
        public List<BlogPost> BlogPosts { get; set; } = new();

        // For Contact Management
        public List<Contact> Contacts { get; set; } = new();

        // For Project Management
        public ProjectInputModel NewProject { get; set; } = default!;
        public List<Project> Projects { get; set; } = new();
    }
}
