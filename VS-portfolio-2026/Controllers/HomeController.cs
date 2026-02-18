using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VS_portfolio_2026.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace VS_portfolio_2026.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMongoDatabase _database;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(ILogger<HomeController> logger, IMongoDatabase database, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _database = database;
        _configuration = configuration;
        _webHostEnvironment = webHostEnvironment;
    }

    // --- Temporary Utility to Upload Resume ---
    // This action should be run once to upload the resume to the database.
    [HttpGet]
    public async Task<IActionResult> UploadResumeToDb()
    {
        try
        {
            var resumeCollectionName = _configuration.GetSection("MongoDbSettings")["ResumeCollectionName"];
            if (string.IsNullOrEmpty(resumeCollectionName))
            {
                return Content("Error: ResumeCollectionName is not configured in appsettings.json.");
            }
            var resumeCollection = _database.GetCollection<ResumeFile>(resumeCollectionName);

            // Clear the collection to ensure only one resume exists
            await resumeCollection.DeleteManyAsync(FilterDefinition<ResumeFile>.Empty);

            // Read the file
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Resume", "Abu-Huraira-Resume.pdf");
            if (!System.IO.File.Exists(filePath))
            {
                return Content("Error: Resume file not found at " + filePath);
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // Create the model
            var resumeFile = new ResumeFile
            {
                FileName = "Abu-Huraira-Resume.pdf",
                Content = fileBytes,
                ContentType = "application/pdf"
            };

            // Insert into MongoDB
            await resumeCollection.InsertOneAsync(resumeFile);

            return Content("Resume uploaded successfully! You can now remove the UploadResumeToDb action from HomeController.cs if you wish.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading resume to database.");
            return Content($"An error occurred: {ex.Message}");
        }
    }
    
    // --- CV Download Action ---
    [HttpGet]
    public async Task<IActionResult> DownloadCv()
    {
        try
        {
            var resumeCollectionName = _configuration.GetSection("MongoDbSettings")["ResumeCollectionName"];
            if (string.IsNullOrEmpty(resumeCollectionName))
            {
                 _logger.LogWarning("ResumeCollectionName is not configured in appsettings.json.");
                return NotFound("Resume functionality is not configured.");
            }
            var resumeCollection = _database.GetCollection<ResumeFile>(resumeCollectionName);

            // Find the first (and only) resume in the collection
            var resumeFile = await resumeCollection.Find(FilterDefinition<ResumeFile>.Empty).FirstOrDefaultAsync();

            if (resumeFile == null || resumeFile.Content == null)
            {
                _logger.LogWarning("Resume file not found in the database.");
                return NotFound("Resume not found in the database.");
            }

            // Return the file to the browser for download
            return File(resumeFile.Content, resumeFile.ContentType, resumeFile.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading resume from database.");
            return StatusCode(500, "An internal error occurred while fetching the resume.");
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

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Portfolio()
    {
        return View();
    }

    public IActionResult Service()
    {
        return View();
    }


    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Blog()
    {
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

