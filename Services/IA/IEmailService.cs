using CatalogoBackend.Models.Responses;
using MimeKit;

namespace CatalogoBackend.Services.IA
{
    public interface IEmailService
    {
        Task<GeneralResponse<string>> SendEmailAsync(string to, string subject, BodyBuilder builder);
    }
}
