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

using Microsoft.AspNetCore.Hosting;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace VS_portfolio_2026.Controllers
{
    [Authorize] // Protect the whole controller
    public class AdminController : Controller
    {
        private readonly IMongoDatabase _database;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IMongoDatabase database, IWebHostEnvironment webHostEnvironment, ILogger<AdminController> logger)
        {
            _database = database;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
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
                var adminCollection = _database.GetCollection<Admin>("Admins");
                var adminUser = await adminCollection.Find(a => a.Username == model.Username).FirstOrDefaultAsync();

                if (adminUser != null && adminUser.Password == model.Password)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, adminUser.Username),
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
        public IActionResult Index()
        {
            var resumePath = Path.Combine(_webHostEnvironment.WebRootPath, "Resume");
            var currentCvFilename = "No CV uploaded yet.";

            if (Directory.Exists(resumePath))
            {
                var pdfFile = Directory.GetFiles(resumePath, "*.pdf").FirstOrDefault();
                if (pdfFile != null)
                {
                    currentCvFilename = Path.GetFileName(pdfFile);
                }
            }

            var model = new AdminDashboardViewModel
            {
                CurrentCvFilename = currentCvFilename
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AdminDashboardViewModel model)
        {
            if (model.NewCvFile != null && model.NewCvFile.Length > 0)
            {
                if (model.NewCvFile.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("NewCvFile", "Please upload a valid PDF file.");
                }
                else
                {
                    var resumePath = Path.Combine(_webHostEnvironment.WebRootPath, "Resume");
                    _logger.LogInformation("Attempting to save resume to path: {path}", resumePath);
                    Directory.CreateDirectory(resumePath); // Ensure the directory exists

                    // Delete any existing PDF files
                    try
                    {
                        var existingFiles = Directory.GetFiles(resumePath, "*.pdf");
                        foreach (var file in existingFiles)
                        {
                            _logger.LogInformation("Deleting existing file: {file}", file);
                            System.IO.File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting existing resume files.");
                        ModelState.AddModelError(string.Empty, "An error occurred while deleting old resume. Check logs.");
                    }

                    // Save the new file
                    try
                    {
                        var newFilePath = Path.Combine(resumePath, model.NewCvFile.FileName);
                        _logger.LogInformation("Saving new file to: {path}", newFilePath);
                        using (var stream = new FileStream(newFilePath, FileMode.Create))
                        {
                            await model.NewCvFile.CopyToAsync(stream);
                        }
                        model.CvUploadSuccessMessage = "Resume uploaded successfully!";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving new resume file.");
                        ModelState.AddModelError(string.Empty, "An error occurred while saving the new resume. Check logs.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("NewCvFile", "Please select a file to upload.");
            }

            // Refresh the current CV filename for the view
            var resumeDir = Path.Combine(_webHostEnvironment.WebRootPath, "Resume");
            var pdf = Directory.Exists(resumeDir) ? Directory.GetFiles(resumeDir, "*.pdf").FirstOrDefault() : null;
            model.CurrentCvFilename = pdf != null ? Path.GetFileName(pdf) : "No CV uploaded yet.";
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
