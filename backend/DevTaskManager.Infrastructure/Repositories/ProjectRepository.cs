using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Enums;
using DevTaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevTaskManager.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>
{
    public ProjectRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(IEnumerable<Project> Items, int Total)> GetPagedAsync(
        int page, int size, string? status, uint? typeId)
    {
        var query = _context.Projects
            .Include(p => p.ProjectType)
            .Include(p => p.Technology)
            .Include(p => p.Creator)
            .Include(p => p.ProjectDevelopers)
                .ThenInclude(pd => pd.Developer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var mapped = status switch {
                "in_progress" => ProjectStatus.InProgress,
                "on_hold"     => ProjectStatus.OnHold,
                _             => Enum.Parse<ProjectStatus>(status, true)
            };
            query = query.Where(p => p.Status == mapped);
        }

        if (typeId.HasValue)
            query = query.Where(p => p.ProjectTypeId == typeId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Project?> GetByIdWithDetailsAsync(uint id) =>
        await _context.Projects
            .Include(p => p.ProjectType)
            .Include(p => p.Technology)
            .Include(p => p.Creator)
            .Include(p => p.ProjectDevelopers)
                .ThenInclude(pd => pd.Developer)
                    .ThenInclude(d => d.ProjectType)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<bool> ExistsByCodeAsync(string code, uint? excludeId = null)
    {
        var q = _context.Projects.Where(p => p.ProjectCode == code);
        if (excludeId.HasValue) q = q.Where(p => p.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<int> GetNextSequenceForYearAsync(int year)
    {
        var prefix = $"PRJ-{year}-";
        var count = await _context.Projects
            .Where(p => p.ProjectCode.StartsWith(prefix))
            .CountAsync();
        return count + 1;
    }

    public async Task<IEnumerable<Developer>> GetDevelopersAsync(uint projectId) =>
        await _context.ProjectDevelopers
            .Where(pd => pd.ProjectId == projectId)
            .Include(pd => pd.Developer)
            .Select(pd => pd.Developer)
            .ToListAsync();
}
