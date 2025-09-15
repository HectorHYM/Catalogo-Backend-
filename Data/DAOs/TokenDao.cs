using CatalogoBackend.Models.Entities;
using CatalogoBackend.Data.IA;
using Microsoft.EntityFrameworkCore;
using CatalogoBackend.Utils;
using CatalogoBackend.Models.DTOs;
using CatalogoBackend.Models.Responses;

namespace CatalogoBackend.Data.DAOs
{
    public class TokenDao : ITokenDao
    {
        private readonly AppDbContext _db;

        public TokenDao(AppDbContext db)
        {
            _db = db;
        }

        public async Task RegisterToken(Token token)
        {
            _db.Tokens.Add(token);
            await _db.SaveChangesAsync();
        }

        public async Task<Token?> ValidateToken(string tokenHash)
        {
            var tokenEntity = await _db.Tokens
                                        .Include(t => t.User)
                                        .FirstOrDefaultAsync(t =>
                                        t.TokenValue == tokenHash &&
                                        t.Type == "access" ||
                                        t.Type == "recover" &&
                                        t.IsActive &&
                                        t.ExpiresAt > DateTime.UtcNow);

            if (tokenEntity == null) return null;

            return tokenEntity;
        }

        public async Task<GeneralResponse<string>> ActivateUser(Token tokenEntity, ActivateDto dto)
        {
            using var trx = await _db.Database.BeginTransactionAsync();
            try
            {
                var user = tokenEntity.User;

                //* Se hashea la contraseña y se activa el usuario
                user.PasswordHash = HashHelper.HashPassword(dto.Password);
                user.IsActive = true;

                //* Se consume el token
                tokenEntity.IsActive = false;
                tokenEntity.UsedAt = DateTime.UtcNow;

                //* Se desactivan los otros tokens del mismo tipo para el usuario activado
                var others = _db.Tokens.Where(t => t.UserId == user.Id && t.Type == "access" && t.IsActive);
                await others.ForEachAsync(t => t.IsActive = false);

                await _db.SaveChangesAsync();
                await trx.CommitAsync();

                return GeneralResponse<string>.Ok(null, "Contraseña establecida."); //? NoContent
            }
            catch
            {
                await trx.RollbackAsync();
                return GeneralResponse<string>.Fail(null, "Error al establecer contraseña y activar cuenta. Intente más tarde.", ResponseCode.ServerError); //* Server Error (500)
            }
        }

        public async Task RefreshTokens(CancellationToken stoppingToken)
        {
            await _db.Database.ExecuteSqlRawAsync("UPDATE tbl_tokens SET is_active = false WHERE is_active = true AND expires_at <= now()", cancellationToken: stoppingToken);
        }
    }
}
