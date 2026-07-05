using DevTaskManager.Application.DTOs.Dashboard;
using DevTaskManager.Application.Interfaces;
using DevTaskManager.Domain.Enums;
using DevTaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Application.Services;

/// <summary>
/// Implementación del servicio de estadísticas para el Dashboard.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        // ── Estadísticas de Proyectos ──────────────────────────────────
        var projects = await _context.Projects.ToListAsync();
        var projectStats = new ProjectStatsDto(
            Total: projects.Count,
            ByStatus: new ProjectByStatusDto(
                Planning:    projects.Count(p => p.Status == ProjectStatus.Planning),
                InProgress:  projects.Count(p => p.Status == ProjectStatus.InProgress),
                OnHold:      projects.Count(p => p.Status == ProjectStatus.OnHold),
                Completed:   projects.Count(p => p.Status == ProjectStatus.Completed),
                Cancelled:   projects.Count(p => p.Status == ProjectStatus.Cancelled)
            )
        );

        // ── Estadísticas de Tareas ─────────────────────────────────────
        var tasks = await _context.Tasks.ToListAsync();
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var taskStats = new TaskStatsDto(
            Total: tasks.Count,
            ByStatus: new TaskByStatusDto(
                Todo:       tasks.Count(t => t.Status == TaskStatus.Todo),
                InProgress: tasks.Count(t => t.Status == TaskStatus.InProgress),
                Review:     tasks.Count(t => t.Status == TaskStatus.Review),
                Done:       tasks.Count(t => t.Status == TaskStatus.Done),
                Blocked:    tasks.Count(t => t.Status == TaskStatus.Blocked)
            ),
            ByPriority: new TaskByPriorityDto(
                Low:      tasks.Count(t => t.Priority == TaskPriority.Low),
                Medium:   tasks.Count(t => t.Priority == TaskPriority.Medium),
                High:     tasks.Count(t => t.Priority == TaskPriority.High),
                Critical: tasks.Count(t => t.Priority == TaskPriority.Critical)
            ),
            CompletedThisMonth: tasks.Count(t =>
                t.Status == TaskStatus.Done &&
                t.CompletedAt.HasValue &&
                t.CompletedAt.Value >= startOfMonth),
            OverdueCount: tasks.Count(t =>
                t.Status != TaskStatus.Done &&
                t.DueDate.HasValue &&
                t.DueDate.Value < DateOnly.FromDateTime(now))
        );

        // ── Estadísticas de Desarrolladores ───────────────────────────
        var developers = await _context.Developers.ToListAsync();
        var devStats = new DeveloperStatsDto(
            Total:  developers.Count,
            Active: developers.Count(d => d.IsActive),
            ByAvailability: new DeveloperByAvailabilityDto(
                Available: developers.Count(d => d.IsActive && d.AvailabilityStatus == AvailabilityStatus.Available),
                Busy:      developers.Count(d => d.IsActive && d.AvailabilityStatus == AvailabilityStatus.Busy),
                Vacation:  developers.Count(d => d.IsActive && d.AvailabilityStatus == AvailabilityStatus.Vacation)
            ),
            BySeniority: new DeveloperBySeniorityDto(
                Junior: developers.Count(d => d.IsActive && d.SeniorityLevel == SeniorityLevel.Junior),
                Mid:    developers.Count(d => d.IsActive && d.SeniorityLevel == SeniorityLevel.Mid),
                Senior: developers.Count(d => d.IsActive && d.SeniorityLevel == SeniorityLevel.Senior)
            )
        );

        return new DashboardStatsDto(
            GeneratedAt: DateTime.UtcNow,
            Projects: projectStats,
            Tasks: taskStats,
            Developers: devStats
        );
    }
}
