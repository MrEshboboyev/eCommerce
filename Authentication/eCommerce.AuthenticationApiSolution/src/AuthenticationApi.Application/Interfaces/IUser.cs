using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interfaces;

public interface IUser
{
    Task<Response> Register(AppUserDTO userDTO);
    Task<Response> Login(LoginDTO loginDTO);
    Task<GetUserDTO> GetUser(int userId);
}
