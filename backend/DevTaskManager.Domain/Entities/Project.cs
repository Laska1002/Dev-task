using DevTaskManager.Domain.Enums;

namespace DevTaskManager.Domain.Entities;

public class Project
{
    public uint Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint ProjectTypeId { get; set; }
    public uint TechnologyId { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public uint CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ProjectType ProjectType { get; set; } = null!;
    public Technology Technology { get; set; } = null!;
    public User Creator { get; set; } = null!;
    public ICollection<ProjectDeveloper> ProjectDevelopers { get; set; } = new List<ProjectDeveloper>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
