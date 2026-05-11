using DevTaskManager.Application.DTOs.Task;
using DevTaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Gets a paginated list of tasks
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10, 
        [FromQuery] uint? projectId = null, [FromQuery] string? status = null, 
        [FromQuery] string? priority = null, [FromQuery] uint? assignedTo = null)
    {
        var (items, total) = await _taskService.GetPagedAsync(page, size, projectId, status, priority, assignedTo);
        return Ok(new { Items = items, Total = total, Page = page, Size = size });
    }

    /// <summary>
    /// Gets a task by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(uint id)
    {
        var task = await _taskService.GetByIdAsync(id);
        return task != null ? Ok(task) : NotFound();
    }

    /// <summary>
    /// Creates a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var createdBy = uint.Parse(userIdStr ?? "0");

        var created = await _taskService.CreateAsync(dto, createdBy);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates a task
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(uint id, [FromBody] UpdateTaskDto dto)
    {
        var updated = await _taskService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Patches only the status of a task
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PatchStatus(uint id, [FromBody] PatchTaskStatusDto dto)
    {
        await _taskService.PatchStatusAsync(id, dto.Status);
        return NoContent();
    }

    /// <summary>
    /// Deletes a task
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(uint id)
    {
        await _taskService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Gets comments for a task
    /// </summary>
    [HttpGet("{id}/comments")]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComments(uint id)
    {
        var comments = await _taskService.GetCommentsAsync(id);
        return Ok(comments);
    }

    /// <summary>
    /// Adds a comment to a task
    /// </summary>
    [HttpPost("{id}/comments")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddComment(uint id, [FromBody] CreateCommentDto dto)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userId = uint.Parse(userIdStr ?? "0");

        var created = await _taskService.AddCommentAsync(id, dto, userId);
        return CreatedAtAction(nameof(GetComments), new { id = id }, created); // Simplified
    }
}
