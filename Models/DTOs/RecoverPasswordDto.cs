using System.ComponentModel.DataAnnotations;

namespace CatalogoBackend.Models.DTOs
{
    public class RecoverPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string TokenType { get; set; } = "recover";
        [Required]
        public string Flow { get; set; } = "recover";
    }
}
