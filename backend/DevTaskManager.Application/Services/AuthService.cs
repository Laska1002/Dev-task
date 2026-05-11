using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevTaskManager.Application.DTOs.Auth;
using DevTaskManager.Application.Interfaces;
using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Enums;
using DevTaskManager.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DevTaskManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IRepository<User> userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var users = await _userRepository.FindAsync(u => u.Email == dto.Email);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Credenciales inválidas."); // In a real app, use custom exceptions

        if (!user.IsActive)
            throw new Exception("Usuario inactivo.");

        var token = GenerateJwtToken(user);
        return new AuthResponseDto(token, user.Id, user.Username, user.Email, user.Role.ToString().ToLower());
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingEmail = await _userRepository.FindAsync(u => u.Email == dto.Email);
        if (existingEmail.Any())
            throw new Exception("El email ya está registrado.");

        var existingUsername = await _userRepository.FindAsync(u => u.Username == dto.Username);
        if (existingUsername.Any())
            throw new Exception("El username ya está en uso.");

        var role = Enum.Parse<UserRole>(dto.Role, true);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var token = GenerateJwtToken(user);
        return new AuthResponseDto(token, user.Id, user.Username, user.Email, user.Role.ToString().ToLower());
    }

    public async Task<AuthResponseDto> GetMeAsync(uint userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var token = GenerateJwtToken(user); // Optional: Refresh token
        return new AuthResponseDto(token, user.Id, user.Username, user.Email, user.Role.ToString().ToLower());
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var expires = int.Parse(jwtSettings["ExpiresInMinutes"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString().ToLower()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expires),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
