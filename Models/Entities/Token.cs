namespace CatalogoBackend.Models.Entities
{
    public class Token
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } //* Clave foranea que corresponde al ID del usuario al que pertenece el token
        public User? User { get; set; } = null!; //* Relación con la entidad User, el usuario al que pertenece el token
        public string TokenValue { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? UsedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}