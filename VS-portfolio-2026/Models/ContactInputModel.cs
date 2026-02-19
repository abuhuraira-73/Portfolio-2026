using System.ComponentModel.DataAnnotations;

namespace VS_portfolio_2026.Models
{
    // This model is used specifically for the contact form binding.
    public class ContactInputModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = null!;

        public string? Subject { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; } = null!;
    }
}
