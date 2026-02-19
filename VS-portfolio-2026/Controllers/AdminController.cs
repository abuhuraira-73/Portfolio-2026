using Microsoft.AspNetCore.Mvc;
using VS_portfolio_2026.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.Extensions.Logging;
using VS_portfolio_2026.Services;

namespace VS_portfolio_2026.Controllers
{
    [Authorize] // Protect the whole controller
    public class AdminController : Controller
    {
        private readonly IDatabaseService _databaseService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IDatabaseService databaseService, IWebHostEnvironment webHostEnvironment, ILogger<AdminController> logger)
        {
            _databaseService = databaseService;
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
                var adminUser = await _databaseService.GetAdminByUsername(model.Username);

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
        public async Task<IActionResult> Index()
        {
            // CV Info
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

            // Education Info
            var educations = await _databaseService.GetEducations();

            // Experience Info
            var experiences = await _databaseService.GetExperiences();

            var model = new AdminDashboardViewModel
            {
                CurrentCvFilename = currentCvFilename,
                Educations = educations,
                NewEducation = new Education(), // Initialize for the form
                Experiences = experiences,
                NewExperience = new ExperienceInputModel() // Initialize for the form
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
        public async Task<IActionResult> AddEducation([Bind(Prefix = "NewEducation")] EducationInputModel input)
        {
            try
            {
                _logger.LogInformation("AddEducation POST received. Data: Year={Year}, Course={Course}, College={College}, Order={Order}",
                    input.Year, input.Course, input.College, input.DisplayOrder);

                if (ModelState.IsValid)
                {
                    _logger.LogInformation("AddEducation ModelState is valid. Creating new Education object and calling database service...");
                    
                    var newEducation = new Education
                    {
                        Year = input.Year,
                        Course = input.Course,
                        College = input.College,
                        Description = input.Description,
                        DisplayOrder = input.DisplayOrder
                    };

                    await _databaseService.AddEducation(newEducation);
                    _logger.LogInformation("Successfully called AddEducation service.");
                }
                else
                {
                    _logger.LogWarning("AddEducation ModelState is invalid. Could not add education entry.");
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning("ModelState Error: Key={Key}, Error={ErrorMessage}", state.Key, error.ErrorMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the education entry.");
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEducation(string id)
        {
            _logger.LogInformation("DeleteEducation POST received for ID: {id}", id);
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("DeleteEducation failed: ID was null or empty.");
                    return RedirectToAction("Index");
                }
                await _databaseService.DeleteEducation(id);
                _logger.LogInformation("Successfully called DeleteEducation service for ID: {id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting education entry with ID: {id}", id);
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExperience([Bind(Prefix = "NewExperience")] ExperienceInputModel input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("AddExperience ModelState is valid. Calling database service...");
                    var newExperience = new Experience
                    {
                        Year = input.Year,
                        Role = input.Role,
                        Company = input.Company,
                        Description = input.Description,
                        DisplayOrder = input.DisplayOrder
                    };
                    await _databaseService.AddExperience(newExperience);
                    _logger.LogInformation("Successfully called AddExperience service.");
                }
                else
                {
                    _logger.LogWarning("AddExperience ModelState is invalid. Could not add experience entry.");
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning("ModelState Error: Key={Key}, Error={ErrorMessage}", state.Key, error.ErrorMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the experience entry.");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExperience(string id)
        {
            _logger.LogInformation("DeleteExperience POST received for ID: {id}", id);
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("DeleteExperience failed: ID was null or empty.");
                    return RedirectToAction("Index");
                }
                await _databaseService.DeleteExperience(id);
                _logger.LogInformation("Successfully called DeleteExperience service for ID: {id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting experience entry with ID: {id}", id);
            }

            return RedirectToAction("Index");
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
