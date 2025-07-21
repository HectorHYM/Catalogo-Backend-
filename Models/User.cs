namespace Catalogo_Backend_.Models
{
    public class User
    {
        //* Guid no-nullable -> Correspondiendo a UUID NOT NULL
        public Guid Id { get; set; }
        public string Name { get; set; } = null!; //* = null! Correspondiendo a NOT NULL
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid? CreatedById { get; set; } //* Clave foranea nullable por ON DELET SET NULL solo con el valor del ID del usuario que creó este otro
        public User? CreatedBy { get; set; } //* Propiedad de navegación a la instancia de usuario completa que creó este otro
        public ICollection<User>? CreatedUsers { get; set; } //* Colección de usuarios creados por otro mismo, para la relación de uno a muchos de forma bidireccional
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public ICollection<User>? UpdatedUsers { get; set; }
    }
}