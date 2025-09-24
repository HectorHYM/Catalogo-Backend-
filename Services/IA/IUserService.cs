using CatalogoBackend.Models.DTOs;
using CatalogoBackend.Models.Entities;
using CatalogoBackend.Models.Responses;

namespace CatalogoBackend.Services.IA
{
    public interface IUserService
    {
        Task<GeneralResponse<UserDto>> Register(UserDto userDto);
        Task<GeneralResponse<string>> Activate(ActivateDto activateDto);
        Task<GeneralResponse<string>> Login(LoginDto loginDto);
    }    
}