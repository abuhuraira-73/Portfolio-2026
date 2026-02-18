using Microsoft.AspNetCore.Mvc;
using VS_portfolio_2026.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.IO;

namespace VS_portfolio_2026.Controllers
{
    [Authorize] // Protect the whole controller
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;

        public AdminController(IConfiguration configuration, IMongoDatabase database)
        {
            _configuration = configuration;
            _database = database;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var adminUsername = _configuration["AdminCredentials:Username"];
                var adminPassword = _configuration["AdminCredentials:Password"];

                if (model.Username == adminUsername && model.Password == adminPassword)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, "Admin"),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var resumeCollection = _database.GetCollection<ResumeFile>(_configuration["MongoDbSettings:ResumeCollectionName"]);
            var resumeFile = await resumeCollection.Find(FilterDefinition<ResumeFile>.Empty).FirstOrDefaultAsync();

            var model = new AdminDashboardViewModel
            {
                CurrentCvFilename = resumeFile?.FileName ?? "No CV uploaded yet."
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AdminDashboardViewModel model)
        {
            var resumeCollection = _database.GetCollection<ResumeFile>(_configuration["MongoDbSettings:ResumeCollectionName"]);

            if (model.NewCvFile != null && model.NewCvFile.Length > 0)
            {
                if (model.NewCvFile.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("NewCvFile", "Please upload a valid PDF file.");
                }
                else
                {
                    // Clear the collection
                    await resumeCollection.DeleteManyAsync(FilterDefinition<ResumeFile>.Empty);

                    // Read file into byte array
                    using var memoryStream = new MemoryStream();
                    await model.NewCvFile.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();

                    // Create new resume file document
                    var newResume = new ResumeFile
                    {
                        FileName = model.NewCvFile.FileName,
                        Content = fileBytes,
                        ContentType = model.NewCvFile.ContentType
                    };

                    // Insert into DB
                    await resumeCollection.InsertOneAsync(newResume);

                    model.CvUploadSuccessMessage = "Resume uploaded successfully!";
                }
            }
            else
            {
                ModelState.AddModelError("NewCvFile", "Please select a file to upload.");
            }

            var resumeFile = await resumeCollection.Find(FilterDefinition<ResumeFile>.Empty).FirstOrDefaultAsync();
            model.CurrentCvFilename = resumeFile?.FileName ?? "No CV uploaded yet.";
            model.NewCvFile = null; // Clear the file input after post

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
