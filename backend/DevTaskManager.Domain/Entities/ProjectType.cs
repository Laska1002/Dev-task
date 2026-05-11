namespace DevTaskManager.Domain.Entities;

public class ProjectType
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
