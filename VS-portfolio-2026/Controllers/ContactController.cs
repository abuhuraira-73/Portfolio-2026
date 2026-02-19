using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VS_portfolio_2026.Models;
using VS_portfolio_2026.Services;

namespace VS_portfolio_2026.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IDatabaseService databaseService, ILogger<ContactController> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        [HttpPost("Submit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([FromForm] ContactInputModel input)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Contact form submission failed validation. Errors: {errors}", string.Join(", ", errors));
                return BadRequest(new { message = "Validation failed.", errors });
            }

            try
            {
                var newContact = new Contact
                {
                    Name = input.Name,
                    Email = input.Email,
                    Subject = input.Subject,
                    Message = input.Message,
                    SubmittedAt = DateTime.UtcNow
                };

                await _databaseService.AddContact(newContact);
                
                _logger.LogInformation("Successfully saved new contact form submission from {name}", newContact.Name);
                
                return Ok(new { message = "Thank you! Your message has been received." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the contact form submission.");
                return StatusCode(500, new { message = "An internal error occurred. Please try again later." });
            }
        }
    }
}
