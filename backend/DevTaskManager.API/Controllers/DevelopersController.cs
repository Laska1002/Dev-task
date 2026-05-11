using DevTaskManager.Application.DTOs.Developer;
using DevTaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DevelopersController : ControllerBase
{
    private readonly IDeveloperService _developerService;

    public DevelopersController(IDeveloperService developerService)
    {
        _developerService = developerService;
    }

    /// <summary>
    /// Gets a paginated list of developers
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? seniority = null,
        [FromQuery] uint? technologyId = null,
        [FromQuery] uint? projectTypeId = null)
    {
        var (items, total) = await _developerService.GetPagedAsync(page, size, status, seniority, technologyId, projectTypeId);
        return Ok(new { Items = items, Total = total, Page = page, Size = size });
    }

    /// <summary>
    /// Gets a developer by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DeveloperResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(uint id)
    {
        var dev = await _developerService.GetByIdAsync(id);
        return dev != null ? Ok(dev) : NotFound();
    }

    /// <summary>
    /// Creates a new developer (Validates Cedula algorithmically)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DeveloperResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDeveloperDto dto)
    {
        var created = await _developerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates a developer (Cedula cannot be changed)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DeveloperResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(uint id, [FromBody] UpdateDeveloperDto dto)
    {
        var updated = await _developerService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Soft deletes a developer
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(uint id)
    {
        await _developerService.SoftDeleteAsync(id);
        return NoContent();
    }
}
