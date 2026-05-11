using DevTaskManager.Domain.Enums;

namespace DevTaskManager.Domain.Entities;

public class Developer
{
    public uint Id { get; set; }
    public uint? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public uint ProjectTypeId { get; set; }
    public SeniorityLevel SeniorityLevel { get; set; } = SeniorityLevel.Mid;
    public AvailabilityStatus AvailabilityStatus { get; set; } = AvailabilityStatus.Available;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public User? User { get; set; }
    public ProjectType ProjectType { get; set; } = null!;
    public ICollection<DeveloperTechnology> DeveloperTechnologies { get; set; } = new List<DeveloperTechnology>();
    public ICollection<ProjectDeveloper> ProjectDevelopers { get; set; } = new List<ProjectDeveloper>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}
