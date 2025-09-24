using CatalogoBackend.Models.DTOs;
using CatalogoBackend.Models.Responses;
using CatalogoBackend.Services.IA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/users")] //?Cambio de ruta a solamente "users"
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    //* Endpoint de registro de usuario
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserDto dto)
    {
        //^LOG
        _logger.LogInformation("Usuario a registrar: {Name}, {Username}, {Email}, {Role}, {IsActive}", dto.Name, dto.Username, dto.Email, dto.Role, dto.IsActive);
        if (!ModelState.IsValid)
        {
            return BadRequest(GeneralResponse<string>.Fail(null, "Error al ingresar usuario, faltan datos."));
        }

        try
        {
            var UserCreated = await _userService.Register(dto);
            //^ Logueo de lo que regresa el método de registro del servicio de usuario.
            _logger.LogInformation("UserService.Register: Code: {Code}, Message: {Message}", UserCreated.Code, UserCreated.Msg);


            //*Se devuelve una respuesta HTTP 201 Created
            switch (UserCreated.Code)
            {
                case ResponseCode.Ok:
                    return Ok(UserCreated);
                case ResponseCode.NoContent:
                    return NoContent();
                case ResponseCode.BadRequest:
                    return BadRequest(UserCreated);
                case ResponseCode.NotFound:
                    return NotFound();
                case ResponseCode.CreatedAtAction:
                    return CreatedAtAction(nameof(Register), UserCreated);
                case ResponseCode.ServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al registrar usuario: {ex}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    //* Endpoint de establecimiento de contraseña y activación de cuenta de usuario
    [HttpPost("activate")]
    [AllowAnonymous]
    public async Task<IActionResult> Activate([FromBody] ActivateDto dto)
    {
        //^LOG
        _logger.LogInformation("Usuario a registrar: {Token}", dto.Token);
        try
        {
            var res = await _userService.Activate(dto);
            _logger.LogInformation("Respuesta del servicio de usuario: {res}", res);

            switch (res.Code)
            {
                case ResponseCode.Ok: 
                    return Ok(res);
                case ResponseCode.NoContent:
                    return NoContent();
                case ResponseCode.BadRequest:
                    return BadRequest(res);
                case ResponseCode.NotFound:
                    return NotFound();
                case ResponseCode.CreatedAtAction:
                    return CreatedAtAction(nameof(Activate), res);
                case ResponseCode.ServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }
        catch (InvalidOperationException ex)
        {
            //^LOG
            _logger.LogError("Error al establecer contraseña y activar usuario: {ex}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(GeneralResponse<string>.Fail(null, "Error al iniciar sesión, faltan datos."));
        }

        try
        {
            var res = await _userService.Login(dto);
            _logger.LogInformation("Respuesta del servicio de usuario para el login: {res}", res);

            switch (res.Code)
            {
                case ResponseCode.Ok:
                    return Ok(res);
                case ResponseCode.NoContent:
                    return NoContent();
                case ResponseCode.BadRequest:
                    return BadRequest(res);
                case ResponseCode.NotFound:
                    return NotFound();
                case ResponseCode.CreatedAtAction:
                    return CreatedAtAction(nameof(Activate), res);
                case ResponseCode.ServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }
        catch(InvalidOperationException ex)
        {
            //^LOG
            _logger.LogError("Error al iniciar sesión: {ex}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}