using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using MimeKit.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace CatalogoBackend.Utils
{
    public class EmailGenerator
    {
        private readonly IConfiguration _conf;
        private readonly IWebHostEnvironment _env;

        public EmailGenerator(IConfiguration conf, IWebHostEnvironment env)
        {
            _conf = conf;
            _env = env;
        }

        //* Se construye URL de confirmación
        public string GenerateConfirmUrl(string rawToken)
        {
            var baseFront = _conf["FrontendBaseUrl"] ? .TrimEnd('/') ?? "https://miapp.com";
            var query = new Dictionary<string, string?>
            {
                ["token"] = rawToken
            };
            var confirmUrl = QueryHelpers.AddQueryString($"{baseFront}/users/password", query); //? http://localhost:4200/password?token=abc-123_XYZ

            return confirmUrl;
        }

        //* Se generan los cuerpos para los correos
        public BodyBuilder GenerateHtmlBody(string type, string username, string confirmUrl = "", string password = "")
        {
            var builder = new BodyBuilder();
            //* Ruta del archivo para la imagen del correo
            var imagePath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "images", "Logo-NoBg.png");
            //* Se agrega el recurso enlazado (inline)
            var image = builder.LinkedResources.Add(imagePath);
            //* Se genera un content-id único (MimeKit)
            image.ContentId = MimeUtils.GenerateMessageId();

            switch (type)
            {
                case "Register":
                    builder.HtmlBody = $@"
                        <html>
                            <body style='font-family: Arial, sans-serif;'>
                                <h2>Bienvenido a catálogo Blxsh</h2>
                                <img src=""cid:{image.ContentId}"" alt=""Logo"" style=""max-width: 300px; max-height: auto;""/>
                                <p>Hola <b>{System.Net.WebUtility.HtmlEncode(username)}</b> desde este link podras establecer tu contraseña y activar tu cuenta, el token envíado tiene validez por solo 30 minutos por lo que en caso de caducar puedes solicitar otro desde la app.</p>
                                <p style='text-align: center;'>
                                    <a href='{confirmUrl}' style='display: inline-block; padding: 12px 20px; border-radius: 6px; background: #B200FF; color: #FFFFFF; text-decoration: none; font-weight: bold;'>
                                        Establecer contraseña.
                                    </a>
                                </p>
                                <p>Si el botón no funciona, copia y pega esta URL en tu navegador:</p>
                                <p><small>{System.Net.WebUtility.HtmlEncode(confirmUrl)}</small></p>
                                <hr>
                                <small>Si no solicitaste este correo, ignóralo.</small>
                            </body>
                        </html>
                    ";
                    break;

                case "Activate":
                    if (File.Exists(imagePath))
                    {
                        builder.HtmlBody = $@"
                            <html>
                                <body style='font-family: Arial, sans-serif;'>
                                    <h2>Bienvenido a catálogo Blxsh</h2>
                                    <img src=""cid:{image.ContentId}"" alt=""Logo"" style=""max-width: 300px; max-height: auto;""/>
                                    <p>Hola {System.Net.WebUtility.HtmlEncode(username)} tu contraseña se ha establecido y ahora tu cuenta esta activa para poder ser uilizada. Por favor evita mostrar tu contraseña a terceros o información personal que pueda poner en peligro la información de cuenta.</p>
                                    <hr>
                                    <p>Su contraseña establecida es: <b>{System.Net.WebUtility.HtmlEncode(password)}</b></p> 
                                    <small>Si no solicitaste este correo, ignóralo.</small>
                                </body>
                            </html>
                        ";
                    }
                    else
                    {
                        builder.HtmlBody = $@"
                            <html>
                                <body style='font-family: Arial, sans-serif;'>
                                    <h2>Bienvenido a catálogo Blxsh</h2>
                                    <p>Hola {System.Net.WebUtility.HtmlEncode(username)} tu contraseña se ha establecido y ahora tu cuenta esta activa para poder ser uilizada. Por favor evita mostrar tu contraseña a terceros o información personal que pueda poner en peligro la información de cuenta.</p>
                                    <hr>
                                    <p>Su contraseña establecida es: <b>{System.Net.WebUtility.HtmlEncode(password)}</b></p> 
                                    <small>Si no solicitaste este correo, ignóralo.</small>
                                </body>
                            </html>
                        ";
                    }
                    break;

                case "Recover":

                    if (File.Exists(imagePath))
                    {
                        builder.HtmlBody = $@"
                            <html>
                                <body style='font-family: Arial, sans-serif;'>
                                    <h2>Establezca su nueva contraseña</h2>
                                    <img src=""cid:{image.ContentId}"" alt=""Logo"" style=""max-width: 300px; max-height: auto;""/>
                                    <p>Hola {System.Net.WebUtility.HtmlEncode(username)} para establecer nuevamente su contraseña vaya al siguiente link:</p>
                                    <p style='text-align: center;'>
                                        <a href='{confirmUrl}' style='display: inline-block; padding: 12px 20px; border-radius: 6px; background: #B200FF; color: #FFFFFF; text-decoration: none; font-weight: bold;'>
                                            Establecer contraseña.
                                        </a>
                                    </p>
                                    <p>Si el botón no funciona, copia y pega esta URL en tu navegador:</p>
                                    <p><small>{System.Net.WebUtility.HtmlEncode(confirmUrl)}</small></p>
                                    <hr>
                                    <small>Si no solicitaste este correo, ignóralo.</small>
                                </body>
                            </html>
                        ";
                    }
                    else
                    {
                        builder.HtmlBody = $@"
                            <html>
                                <body style='font-family: Arial, sans-serif;'>
                                    <h2>Establezca su nueva contraseña</h2>
                                    <p>Hola {System.Net.WebUtility.HtmlEncode(username)} para establecer nuevamente su contraseña vaya al siguiente link:</p>
                                    <p style='text-align: center;'>
                                        <a href='{confirmUrl}' style='display: inline-block; padding: 12px 20px; border-radius: 6px; background: #B200FF; color: #FFFFFF; text-decoration: none; font-weight: bold;'>
                                            Establecer contraseña.
                                        </a>
                                    </p>
                                    <p>Si el botón no funciona, copia y pega esta URL en tu navegador:</p>
                                    <p><small>{System.Net.WebUtility.HtmlEncode(confirmUrl)}</small></p>
                                    <hr>
                                    <small>Si no solicitaste este correo, ignóralo.</small>
                                </body>
                            </html>
                        ";
                    }
                    break;
            }
            return builder;
        }
    }
}
