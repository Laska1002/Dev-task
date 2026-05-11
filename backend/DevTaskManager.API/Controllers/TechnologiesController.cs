using DevTaskManager.Domain.Entities;
using DevTaskManager.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TechnologiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TechnologiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] uint? projectTypeId = null)
    {
        var query = _context.Technologies.Where(t => t.IsActive);
        
        if (projectTypeId.HasValue)
        {
            query = query.Where(t => t.ProjectTypeId == projectTypeId.Value);
        }

        var items = await query.OrderBy(t => t.Name).ToListAsync();
        
        var dtos = items.Select(t => new {
            t.Id,
            t.Name,
            t.ProjectTypeId
        });

        return Ok(dtos);
    }
}
