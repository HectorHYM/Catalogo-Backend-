using CatalogoBackend.Services.IA;
using CatalogoBackend.Utils;
using CatalogoBackend.Models.Entities;
using CatalogoBackend.Data.IA;

namespace CatalogoBackend.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _conf;
        private readonly ITokenDao _tokenDao;

        public TokenService(IConfiguration conf, ITokenDao tokenDao)
        {
            _conf = conf;
            _tokenDao = tokenDao;
        }

        public async Task RegisterToken(string rawToken, Guid userId)
        {
            var secret = _conf["Token:Secret"];
            var hashToken = HashHelper.HashTokenHex(rawToken, secret);

            var token = new Token
            {
                UserId = userId,
                TokenValue = hashToken,
                Type = "access",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            await _tokenDao.RegisterToken(token);
        }
    }
}
