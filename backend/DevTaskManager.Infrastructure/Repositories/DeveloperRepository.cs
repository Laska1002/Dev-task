using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Enums;
using DevTaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevTaskManager.Infrastructure.Repositories;

public class DeveloperRepository : Repository<Developer>
{
    public DeveloperRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(IEnumerable<Developer> Items, int Total)> GetPagedAsync(
        int page, int size, string? status, string? seniority, uint? technologyId = null, uint? projectTypeId = null)
    {
        var query = _context.Developers
            .Include(d => d.User)
            .Include(d => d.ProjectType)
            .Include(d => d.DeveloperTechnologies).ThenInclude(dt => dt.Technology)
            .Where(d => d.IsActive);

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<AvailabilityStatus>(status, true, out var s))
            query = query.Where(d => d.AvailabilityStatus == s);

        if (!string.IsNullOrWhiteSpace(seniority) &&
            Enum.TryParse<SeniorityLevel>(seniority, true, out var sl))
            query = query.Where(d => d.SeniorityLevel == sl);

        if (technologyId.HasValue)
            query = query.Where(d => d.DeveloperTechnologies.Any(dt => dt.TechnologyId == technologyId.Value));
            
        if (projectTypeId.HasValue)
            query = query.Where(d => d.ProjectTypeId == projectTypeId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.FullName)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Developer?> GetByIdWithUserAsync(uint id) =>
        await _context.Developers
            .Include(d => d.User)
            .Include(d => d.ProjectType)
            .Include(d => d.DeveloperTechnologies).ThenInclude(dt => dt.Technology)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<bool> ExistsByCedulaAsync(string cedula, uint? excludeId = null)
    {
        var q = _context.Developers.Where(d => d.Cedula == cedula);
        if (excludeId.HasValue) q = q.Where(d => d.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Developers.CountAsync();
        return count + 1;
    }
}
