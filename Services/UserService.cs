using System.Security.Claims;
using CatalogoBackend.Data.IA;
using CatalogoBackend.Models.DTOs;
using CatalogoBackend.Models.Entities;
using CatalogoBackend.Models.Responses;
using CatalogoBackend.Services.IA;
using CatalogoBackend.Utils;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace CatalogoBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly ITokenService _tokenService;
        private readonly EmailGenerator _emailGenerator;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _conf;
        private readonly ITokenDao _tokenDao;
        private readonly JwtGenerator _jwtGenerator;

        public UserService(IUserDao userDao, ITokenService tokenService, EmailGenerator emailGenerator, IEmailService emailService, ILogger<UserService> logger, IConfiguration conf, ITokenDao tokenDao, JwtGenerator jwtGenerator)
        {
            _userDao = userDao;
            _tokenService = tokenService;
            _emailGenerator = emailGenerator;
            _emailService = emailService;
            _logger = logger;
            _conf = conf;
            _tokenDao = tokenDao;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<GeneralResponse<UserDto>> Register(UserDto dto)
        {
            //* Validaciones de negocio
            //* Se asegura que el usuario a registrar no contenga un correo o nombre de usuario en uso
            if (await _userDao.GetByUsername(dto.Username) != null || await _userDao.GetByEmail(dto.Email) != null)
            {
                return GeneralResponse<UserDto>.Fail(null, "Nombre de usuario o correo ya existente.");
            }

            if (dto.Role != "client" && dto.Role != "admin") return GeneralResponse<UserDto>.Fail(null, "Rol no validado, intente más tarde.", ResponseCode.ServerError);

            //* Se mapea el DTO a la entidad POCO
            var user = new User
            {
                Name = dto.Name,
                Username = dto.Username,
                Email = dto.Email,
                Role = dto.Role,
                IsActive = dto.IsActive
            };

            //* Se registra el usuario en la BD
            var createdUser = await _userDao.CreateUser(user);
            //* Se genera token crudo
            var rawToken = HashHelper.GenerateToken();

            if (createdUser != null)
            {
                //* Se registra el nuevo token de usuario para activar la cuenta
                await _tokenService.RegisterToken(rawToken, createdUser.Id, dto.TokenType);
            }
            else
            {
                return GeneralResponse<UserDto>.Fail(null, "Error. No se pudo registrar el usuario. Intente más tarde");
            }

            //* Se generá URL para enviar al correo del usuario con su token crudo
            var confirmUrl = _emailGenerator.GenerateConfirmUrl(rawToken);
            //* Se generá el cuerpo del correo
            var builderHtmlBody = _emailGenerator.GenerateHtmlBody("Register", createdUser.Username, confirmUrl);

            //^ Se envía correo (idealmente en background job, pero por el momento se hace directamente)
            try
            {
                await _emailService.SendEmailAsync(user.Email, "Establecimiento de contraseña", builderHtmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email de confirmación a {Email}", user.Email);
            }

            return GeneralResponse<UserDto>.Ok(new UserDto { Name = createdUser.Name, Username = createdUser.Username, Email = createdUser.Email, Role = createdUser.Role  }, "Usuario registrado exitosamente.", ResponseCode.CreatedAtAction);
        }

        public async Task<GeneralResponse<string>> Activate(ActivateDto dto)
        {
            if(string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return GeneralResponse<string>.Fail(null, "Error: Token/Contraseña no encontrados"); //? BadRequest
                
            }

            var secret = _conf["Token:Secret"];
            var tokenHash = HashHelper.HashTokenHex(dto.Token, secret);

            //* Se busca el token válido 
            var tokenEntity = await _tokenDao.ValidateToken(tokenHash);

            if (tokenEntity != null)
            {
                //* Se generá el cuerpo del correo
                var htmlBody = _emailGenerator.GenerateHtmlBody("Activate", tokenEntity.User.Username, "", dto.Password);

                //^ Se envía correo (idealmente en background job, pero por el momento se hace directamente)
                try
                {
                    await _emailService.SendEmailAsync(tokenEntity.User.Email, "Establecimiento de contraseña/Activación de cuenta", htmlBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error enviando email de confirmación a {Email}", tokenEntity.User.Email);
                }

                return await _tokenDao.ActivateUser(tokenEntity, dto);
            }
            else
            {
                return GeneralResponse<string>.Fail(null, "Ocurrio un error, token expirado"); //? BadRequest
            }
        }

        public async Task<GeneralResponse<string>> Login(LoginDto dto)
        {
            //*Validaciones para el inicio de sesión

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return GeneralResponse<string>.Fail(null, "Error: Datos de inicio de sesión no proporcionados"); //? BadRequest

            }

            var user = await _userDao.GetByEmail(dto.Email);

            var hashedPassDb = user?.PasswordHash;
            if (hashedPassDb == null) return GeneralResponse<string>.Fail(null, "Usuario no encontrado, revise sus credenciales.", ResponseCode.NotFound);

            var validatePassword = HashHelper.VerifyPassword(hashedPassDb, dto.Password);
            if (validatePassword == false) return GeneralResponse<string>.Fail(null, "Contraseña incorrecta para este usuario.");

            if (user?.Role != "client" && user?.Role != "admin") return GeneralResponse<string>.Fail(null, "Rol no validado.", ResponseCode.ServerError);

            //* Contrucción del JWT

            var tokenString = _jwtGenerator.GenerateBearerJwt(user);

            if( tokenString == null)
            {
                return GeneralResponse<string>.Fail(tokenString, "Error al generar token de sesión, intente más tarde.", ResponseCode.ServerError);
            }

            return GeneralResponse<string>.Ok(tokenString, "Inicio correcto de sesión.");
        }

        //* Método para recuperar contraseña en caso de olvidarla
        public async Task<GeneralResponse<string>> RecoverPassword(string email, string type)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return GeneralResponse<string>.Fail(null, "No se proporciono ningún correo electrónico.");
            }

            var rawToken = HashHelper.GenerateToken();
            var user = await _userDao.GetByEmail(email);

            if(user != null)
            {
                try
                {
                    await _tokenService.RegisterToken(rawToken, user.Id, type);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error al registrar el token en la DB: {ex}", ex);
                    return GeneralResponse<string>.Fail(null, "Error, intente más tarde", ResponseCode.ServerError);
                }
            }
            else
            {
                return GeneralResponse<string>.Fail(null, "Usuario no encontrado", ResponseCode.NotFound);
            }

            //* Se generá URL para enviar al correo del usuario con su token crudo
            var confirmUrl = _emailGenerator.GenerateConfirmUrl(rawToken);
            //* Se generá el cuerpo del correo
            var builderHtmlBody = _emailGenerator.GenerateHtmlBody("Recover", user.Username, confirmUrl);

            //^ Se envía correo (idealmente en background job, pero por el momento se hace directamente)
            try
            {
                await _emailService.SendEmailAsync(user.Email, "Recuperación de contraseña", builderHtmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email de confirmación a {Email}", user.Email);
            }

            return GeneralResponse<string>.Ok(null, "Correo envíado exitosamente");
        }

        public async Task<GeneralResponse<UserDto>> GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("No se encontró ningún userId en las claims del usuario.");
                return GeneralResponse<UserDto>.Fail(null, "Usuario no encontrado.", ResponseCode.Unauthorized);
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                _logger.LogError("Claim user id no es convertible a Guid: {userId}", userId);
                return GeneralResponse<UserDto>.Fail(null, "Autorización denegada.", ResponseCode.Unauthorized);
            }

            var user = await _userDao.GetById(userGuid);
            if (user == null)
            {
                return GeneralResponse<UserDto>.Fail(null, "Usuario no encontrado.", ResponseCode.NotFound);
            }

            var dto = new UserDto { Name = user.Name, Username = user.Username, Email = user.Email, IsActive = user.IsActive, Role = user.Role };
            return GeneralResponse<UserDto>.Ok(dto, "Ok");
        }
    }
}