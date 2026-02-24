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
        ViewData["Title"] = "Home";
        ViewData["Description"] = "Personal portfolio of Abu Huraira, a web and software developer specializing in Java, Python, C# and building scalable, high-performance digital products.";
        ViewData["Keywords"] = "Abu Huraira, portfolio, personal portfolio, web developer, software engineer, Java developer, .NET developer, C# developer, React developer";
        ViewData["OgUrl"] = "http://abuhuraira.in/";
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> About()
    {
        ViewData["Title"] = "About";
        ViewData["Description"] = "Learn about Abu Huraira's journey, skills in full-stack development, and experience with technologies like .NET, C#, and modern web frameworks.";
        ViewData["Keywords"] = "about me, Abu Huraira, skills, experience, full-stack, software engineer, C# developer";
        ViewData["OgUrl"] = "http://abuhuraira.in/Home/About";

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
        ViewData["Title"] = "Portfolio";
        ViewData["Description"] = "Explore a collection of projects by Abu Huraira, showcasing skills in web development, software engineering, and more.";
        ViewData["Keywords"] = "projects, portfolio, web development, software projects, C#, .NET, React";
        ViewData["OgUrl"] = "http://abuhuraira.in/Home/Portfolio";
        var projects = await _databaseService.GetProjects();
        return View(projects);
    }

    public IActionResult Service()
    {
        ViewData["Title"] = "Services";
        ViewData["Description"] = "Discover the professional services offered by Abu Huraira, including custom web application development, backend API design, and software consulting.";
        ViewData["Keywords"] = "services, web development, software consulting, API development, C#, ASP.NET Core, freelance";
        ViewData["OgUrl"] = "http://abuhuraira.in/Home/Service";
        return View();
    }


    public IActionResult Contact()
    {
        ViewData["Title"] = "Contact";
        ViewData["Description"] = "Get in touch with Abu Huraira to discuss a project, ask a question, or say hello. Looking forward to connecting with you.";
        ViewData["Keywords"] = "contact, get in touch, contact me, email, contact form, hire me";
        ViewData["OgUrl"] = "http://abuhuraira.in/Home/Contact";
        return View();
    }

    public async Task<IActionResult> Blog()
    {
        ViewData["Title"] = "Blog";
        ViewData["Description"] = "Read articles and posts from Abu Huraira on software development, technology trends, and career insights.";
        ViewData["Keywords"] = "blog, articles, tech blog, software development, coding, programming, career advice";
        ViewData["OgUrl"] = "http://abuhuraira.in/Home/Blog";
        var blogPosts = await _databaseService.GetBlogPosts();
        return View(blogPosts);
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

