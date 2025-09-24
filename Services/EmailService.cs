using CatalogoBackend.Models.Entities;
using CatalogoBackend.Models.Responses;
using CatalogoBackend.Services.IA;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CatalogoBackend.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _conf;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IOptions<SmtpSettings> opts, ILogger<EmailService> logger)
        {
            _conf = opts.Value;
            _logger = logger;
        }

        public async Task<GeneralResponse<string>> SendEmailAsync(string to, string subject, BodyBuilder builder)
        {
            //* Construcción del correo (encabezado, destinatario, asunto, etc...)
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_conf.FromName, _conf.From));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;

            //* Se construye el cuerpo del correo en 2 versiones (HTML y Texto plano)
            builder.TextBody = StripHtmlToText(builder.HtmlBody);
            msg.Body = builder.ToMessageBody();

            //* Conexión con Smtp con disposable automático
            using var client = new SmtpClient();

            try
            {
                //* Conexión con TLS (Transport Layer Security)
                await client.ConnectAsync(_conf.Host, _conf.Port, SecureSocketOptions.StartTls);
                if (!string.IsNullOrEmpty(_conf.User) || !string.IsNullOrEmpty(_conf.Pass))
                {
                    await client.AuthenticateAsync(_conf.User, _conf.Pass);
                }
                else
                {
                    _logger.LogWarning("Credenciales no proporcionadas para la conexión SMTP. Intentando envío sin autenticación");
                    if(client.AuthenticationMechanisms != null && client.AuthenticationMechanisms.Any())
                    {
                        _logger.LogError("El servidor SMTP requiere autenticación pero no hay credenciales proporcionadas");
                        return GeneralResponse<string>.Fail(null, "El servidor SMTP requiere autenticación pero no hay credenciales proporcionadas");
                    }
                }

                //* Envío de mensaje
                await client.SendAsync(msg);
                _logger.LogInformation("Correo envíado a {To}", to);
                return GeneralResponse<string>.Ok(null, "Mensaje envíado a su correo electrónico.", ResponseCode.NoContent);
            }
            catch(SmtpCommandException scEx)
            {
                _logger.LogError(scEx, "Smtp error de comando: {Message}", scEx.Message);
                return GeneralResponse<string>.Fail(null, "Smtp error de comando", ResponseCode.ServerError);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error en el envío del correo: {message}", ex.Message);
                return GeneralResponse<string>.Fail(null, "Error en el envío del correo", ResponseCode.ServerError);
            }
            finally
            {
                if (client.IsConnected)
                {
                    try
                    {
                        await client.DisconnectAsync(true);
                    }
                    catch (Exception ex) 
                    {
                        _logger.LogWarning(ex, "Error desconectando el cliente SMTP: {message}", ex.Message);
                    }
                }
            }
        }

        //* Convertor de HTML a Texto plano
        private string StripHtmlToText(string htmlBody)
        {
            return System.Text.RegularExpressions.Regex.Replace(htmlBody, "<.*?>", string.Empty);
        }
    }
}
