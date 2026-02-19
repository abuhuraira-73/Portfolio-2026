using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VS_portfolio_2026.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VS_portfolio_2026.Services;

namespace VS_portfolio_2026.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IDatabaseService _databaseService;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, IDatabaseService databaseService)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        _databaseService = databaseService;
    }

    // --- CV Download Action ---
    [HttpGet]
    public async Task<IActionResult> DownloadCv()
    {
        try
        {
            var resumePath = Path.Combine(_webHostEnvironment.WebRootPath, "Resume");
            if (!Directory.Exists(resumePath))
            {
                _logger.LogWarning("Resume directory not found at {path}", resumePath);
                return NotFound("Resume directory not found.");
            }

            var pdfFile = Directory.GetFiles(resumePath, "*.pdf").FirstOrDefault();

            if (pdfFile == null)
            {
                _logger.LogWarning("No resume file found in the directory.");
                return NotFound("Resume not found.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfFile);
            var fileName = Path.GetFileName(pdfFile);

            // Return the file to the browser for download
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing resume download.");
            return StatusCode(500, "An internal error occurred while processing the resume.");
        }
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> About()
    {
        var educations = await _databaseService.GetEducations();
        var experiences = await _databaseService.GetExperiences();

        var viewModel = new AboutPageViewModel
        {
            Educations = educations,
            Experiences = experiences
        };

        return View(viewModel);
    }


    public async Task<IActionResult> Portfolio()
    {
        var projects = await _databaseService.GetProjects();
        return View(projects);
    }

    public IActionResult Service()
    {
        return View();
    }


    public IActionResult Contact()
    {
        return View();
    }

    public async Task<IActionResult> Blog()
    {
        var blogPosts = await _databaseService.GetBlogPosts();
        return View(blogPosts);
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

