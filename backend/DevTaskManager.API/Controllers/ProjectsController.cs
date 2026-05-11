using DevTaskManager.Application.DTOs.Project;
using DevTaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Gets a paginated list of projects
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10, 
        [FromQuery] string? status = null, [FromQuery] uint? typeId = null)
    {
        var (items, total) = await _projectService.GetPagedAsync(page, size, status, typeId);
        return Ok(new { Items = items, Total = total, Page = page, Size = size });
    }

    /// <summary>
    /// Gets a project by ID with details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(uint id)
    {
        var project = await _projectService.GetByIdAsync(id);
        return project != null ? Ok(project) : NotFound();
    }

    /// <summary>
    /// Creates a new project
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var createdBy = uint.Parse(userIdStr ?? "0");

        var created = await _projectService.CreateAsync(dto, createdBy);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates a project
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(uint id, [FromBody] UpdateProjectDto dto)
    {
        var updated = await _projectService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a project
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(uint id)
    {
        await _projectService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Assigns developers to a project
    /// </summary>
    [HttpPost("{id}/developers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignDevelopers(uint id, [FromBody] AssignDevelopersDto dto)
    {
        await _projectService.AssignDevelopersAsync(id, dto);
        return Ok();
    }

    /// <summary>
    /// Removes a developer from a project
    /// </summary>
    [HttpDelete("{id}/developers/{devId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveDeveloper(uint id, uint devId)
    {
        await _projectService.RemoveDeveloperAsync(id, devId);
        return NoContent();
    }

    /// <summary>
    /// Gets developers assigned to a project
    /// </summary>
    [HttpGet("{id}/developers")]
    [ProducesResponseType(typeof(IEnumerable<ProjectDeveloperDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDevelopers(uint id)
    {
        var devs = await _projectService.GetDevelopersAsync(id);
        return Ok(devs);
    }
}
