using System.ComponentModel.DataAnnotations;

//* DTO para registro de usuarios */
namespace CatalogoBackend.Models.DTOs
{
    public class UserDto
    {
        [Required]
        [MaxLength(50)]
        [MinLength(7)]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        public string Username { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Role { get; set; } = null!;
        [Required]
        public bool IsActive { get; set; } = false;
    }
}