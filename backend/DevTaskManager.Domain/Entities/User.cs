using DevTaskManager.Domain.Enums;

namespace DevTaskManager.Domain.Entities;

public class User
{
    public uint Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Developer;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<Developer> Developers { get; set; } = new List<Developer>();
    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
}
