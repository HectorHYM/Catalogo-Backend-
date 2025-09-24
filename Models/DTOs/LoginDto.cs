using System.ComponentModel.DataAnnotations;

namespace CatalogoBackend.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;
    }
}
