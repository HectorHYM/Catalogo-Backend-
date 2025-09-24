using CatalogoBackend.Models.DTOs;
using CatalogoBackend.Models.Entities;
using CatalogoBackend.Models.Responses;

namespace CatalogoBackend.Data.IA
{
    public interface ITokenDao
    {
        Task RegisterToken(Token hashToken);
        Task<Token?> ValidateToken(string tokenHash);
        Task<GeneralResponse<string>> ActivateUser(Token tokenUserEntity, ActivateDto dto);
        Task RefreshTokens(CancellationToken stoppingToken);
    }
}
