namespace Catalogo_Backend_.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImgUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid? CreatedById { get; set; } //* Clave foranea nullable por ON DELET SET NULL solo con el valor del ID del usuario que creó este otro
        public Product? CreatedBy { get; set; } //* Propiedad de navegación a la instancia de usuario completa que creó este otro
        public ICollection<Product>? CreatedProducts { get; set; } //* Colección de usuarios creados por otro mismo, para la relación de uno a muchos de forma bidireccional
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid? UpdatedById { get; set; }
        public Product? UpdatedBy { get; set; }
        public ICollection<Product>? UpdatedProducts { get; set; } //* Colección de usuarios creados por otro mismo, para la relación de uno a muchos de forma bidireccional
    }
}