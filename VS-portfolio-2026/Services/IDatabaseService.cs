using System.Collections.Generic;
using System.Threading.Tasks;
using VS_portfolio_2026.Models;

namespace VS_portfolio_2026.Services
{
    public interface IDatabaseService
    {
        Task<Admin?> GetAdminByUsername(string username);
        Task<List<Education>> GetEducations();
        Task AddEducation(Education education);
        Task DeleteEducation(string id);

        Task<List<Experience>> GetExperiences();
        Task AddExperience(Experience experience);
        Task DeleteExperience(string id);

        Task<List<BlogPost>> GetBlogPosts();
        Task AddBlogPost(BlogPost blogPost);
        Task DeleteBlogPost(string id);
    }
}
