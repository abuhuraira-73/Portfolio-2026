using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using VS_portfolio_2026.Models;

namespace VS_portfolio_2026.Services
{
    public class MongoDbService : IDatabaseService
    {
        private readonly IMongoCollection<Admin> _admins;
        private readonly IMongoCollection<Education> _educations;
        private readonly IMongoCollection<Experience> _experiences;
        private readonly IMongoCollection<BlogPost> _blogPosts;
        private readonly IMongoCollection<Contact> _contacts;
        private readonly IMongoCollection<Project> _projects;

        public MongoDbService(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("MongoDbSettings")["ConnectionString"];
            var databaseName = configuration.GetSection("MongoDbSettings")["DatabaseName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _admins = database.GetCollection<Admin>("Admins");
            _educations = database.GetCollection<Education>("Educations");
            _experiences = database.GetCollection<Experience>("Experiences");
            _blogPosts = database.GetCollection<BlogPost>("BlogPosts");
            _contacts = database.GetCollection<Contact>("Contacts");
            _projects = database.GetCollection<Project>("Projects");
        }

        public async Task<Admin?> GetAdminByUsername(string username)
        {
            return await _admins.Find(a => a.Username == username).FirstOrDefaultAsync();
        }

        public async Task<List<Education>> GetEducations()
        {
            return await _educations.Find(FilterDefinition<Education>.Empty).SortBy(e => e.DisplayOrder).ToListAsync();
        }

        public async Task AddEducation(Education education)
        {
            await _educations.InsertOneAsync(education);
        }

        public async Task DeleteEducation(string id)
        {
            await _educations.DeleteOneAsync(e => e.Id == id);
        }

        public async Task<List<Experience>> GetExperiences()
        {
            return await _experiences.Find(FilterDefinition<Experience>.Empty).SortBy(e => e.DisplayOrder).ToListAsync();
        }

        public async Task AddExperience(Experience experience)
        {
            await _experiences.InsertOneAsync(experience);
        }

        public async Task DeleteExperience(string id)
        {
            await _experiences.DeleteOneAsync(e => e.Id == id);
        }

        public async Task<List<BlogPost>> GetBlogPosts()
        {
            return await _blogPosts.Find(FilterDefinition<BlogPost>.Empty).SortBy(b => b.DisplayOrder).ToListAsync();
        }

        public async Task AddBlogPost(BlogPost blogPost)
        {
            await _blogPosts.InsertOneAsync(blogPost);
        }

        public async Task DeleteBlogPost(string id)
        {
            await _blogPosts.DeleteOneAsync(b => b.Id == id);
        }

        public async Task AddContact(Contact contact)
        {
            await _contacts.InsertOneAsync(contact);
        }

        public async Task<List<Contact>> GetContacts()
        {
            return await _contacts.Find(FilterDefinition<Contact>.Empty).SortByDescending(c => c.SubmittedAt).ToListAsync();
        }

        public async Task DeleteContact(string id)
        {
            await _contacts.DeleteOneAsync(c => c.Id == id);
        }

        public async Task<List<Project>> GetProjects()
        {
            return await _projects.Find(FilterDefinition<Project>.Empty).SortBy(p => p.DisplayOrder).ToListAsync();
        }

        public async Task AddProject(Project project)
        {
            await _projects.InsertOneAsync(project);
        }

        public async Task DeleteProject(string id)
        {
            await _projects.DeleteOneAsync(p => p.Id == id);
        }
    }
}
