using System.ComponentModel.DataAnnotations;

//* DTO para activación de cuenta de usuario */
namespace CatalogoBackend.Models.DTOs
{
    public class ActivateDto
    {
        [Required]
        public string Token { get; set; } = null!;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;
    }
}
