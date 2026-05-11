using DevTaskManager.Application.DTOs.Auth;
using DevTaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return Ok(response);
    }

    /// <summary>
    /// Registers a new user. Only admins can register new users.
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var response = await _authService.RegisterAsync(dto);
        return Ok(response);
    }

    /// <summary>
    /// Gets information about the currently authenticated user
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !uint.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var response = await _authService.GetMeAsync(userId);
        return Ok(response);
    }
}
