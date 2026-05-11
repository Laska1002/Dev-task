using DevTaskManager.Application.DTOs.Auth;

namespace DevTaskManager.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> GetMeAsync(uint userId);
}
