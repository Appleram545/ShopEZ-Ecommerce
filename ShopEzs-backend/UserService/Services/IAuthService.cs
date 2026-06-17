using UserService.DTOs;

namespace UserService.Services;

public interface IAuthService
{
    Task<string> Register(RegisterDto dto);

    Task<string> Login(LoginDto dto);
}