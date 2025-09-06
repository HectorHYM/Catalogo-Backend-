namespace CatalogoBackend.Services.IA
{
    public interface ITokenService
    {
        Task RegisterToken(string token, Guid userId);
    }
}
