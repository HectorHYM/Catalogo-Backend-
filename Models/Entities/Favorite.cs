namespace CatalogoBackend.Models.Entities
{
    public class Favorite
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!; // Relación a la entidad de usuarios.
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!; // Relación a la entidad de productos.
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
