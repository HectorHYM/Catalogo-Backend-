using System.Security.Claims;
using CatalogoBackend.Models.DTOs;
using CatalogoBackend.Models.Responses;

namespace CatalogoBackend.Services.IA
{
    public interface IUserService
    {
        Task<GeneralResponse<UserDto>> Register(UserDto userDto);
        Task<GeneralResponse<string>> Activate(ActivateDto activateDto);
        Task<GeneralResponse<string>> Login(LoginDto loginDto);
        Task<GeneralResponse<string>> RecoverPassword(string email, string type);
        Task<GeneralResponse<UserDto>> GetUser(string ?userId);
    }    
}