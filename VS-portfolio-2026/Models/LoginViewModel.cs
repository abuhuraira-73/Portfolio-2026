using System.ComponentModel.DataAnnotations;

namespace VS_portfolio_2026.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
