namespace DevTaskManager.Domain.Entities;

public class ProjectDeveloper
{
    public uint Id { get; set; }
    public uint ProjectId { get; set; }
    public uint DeveloperId { get; set; }
    public string? RoleInProject { get; set; }
    public DateTime AssignedAt { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public Developer Developer { get; set; } = null!;
}
