using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Enums;
using DevTaskManager.Domain.Interfaces;
using DevTaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Infrastructure.Repositories;

/// <summary>
/// PATRÓN REPOSITORY — Implementación concreta del repositorio de tareas.
/// SOLID - DIP: Implementa ITaskRepository (definida en Domain), permitiendo que TaskService dependa
///              de la interfaz y no de esta clase concreta.
/// </summary>
public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(IEnumerable<TaskItem> Items, int Total)> GetPagedAsync(
        int page, int size, uint? projectId, string? status, string? priority, uint? assignedTo)
    {
        var query = _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedDeveloper)
            .Include(t => t.Creator)
            .AsQueryable();

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status == "in_progress" ? TaskStatus.InProgress
                  : Enum.Parse<TaskStatus>(status, true);
            query = query.Where(t => t.Status == s);
        }

        if (!string.IsNullOrWhiteSpace(priority) &&
            Enum.TryParse<TaskPriority>(priority, true, out var p))
            query = query.Where(t => t.Priority == p);

        if (assignedTo.HasValue)
            query = query.Where(t => t.AssignedTo == assignedTo.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, total);
    }

    public async Task<TaskItem?> GetByIdWithDetailsAsync(uint id) =>
        await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedDeveloper)
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<TaskComment>> GetCommentsAsync(uint taskId) =>
        await _context.TaskComments
            .Include(c => c.User)
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

    public async Task<int> GetNextSequenceForYearAsync(int year)
    {
        var prefix = $"TSK-{year}-";
        var count = await _context.Tasks
            .Where(t => t.TaskCode.StartsWith(prefix))
            .CountAsync();
        return count + 1;
    }
}
