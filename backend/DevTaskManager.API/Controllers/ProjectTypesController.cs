using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTaskManager.API.Controllers;

[ApiController]
[Route("api/project-types")]
[Authorize]
public class ProjectTypesController : ControllerBase
{
    private readonly IRepository<ProjectType> _projectTypeRepository;

    public ProjectTypesController(IRepository<ProjectType> projectTypeRepository)
    {
        _projectTypeRepository = projectTypeRepository;
    }

    /// <summary>
    /// Gets all active project types
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectType>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var types = await _projectTypeRepository.FindAsync(pt => pt.IsActive);
        return Ok(types);
    }
}
