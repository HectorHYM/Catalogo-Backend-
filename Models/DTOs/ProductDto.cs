using System.ComponentModel.DataAnnotations;

namespace CatalogoBackend.Models.DTOs
{
    public class ProductDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock {  get; set; }
        public string ImgUrl { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
