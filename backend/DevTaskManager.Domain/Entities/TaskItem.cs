using DevTaskManager.Domain.Enums;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Domain.Entities;

public class TaskItem
{
    public uint Id { get; set; }
    public string TaskCode { get; set; } = string.Empty;
    public uint ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint? AssignedTo { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public decimal? EstimatedHours { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public uint CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public Developer? AssignedDeveloper { get; set; }
    public User Creator { get; set; } = null!;
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
}
