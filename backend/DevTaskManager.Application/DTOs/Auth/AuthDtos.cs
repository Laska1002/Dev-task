namespace DevTaskManager.Application.DTOs.Auth;

public record LoginDto(string Email, string Password);

public record RegisterDto(string Username, string Email, string Password, string Role);

public record AuthResponseDto(string Token, uint UserId, string Username, string Email, string Role);
