namespace DevTaskManager.Domain.Entities;

public class TaskComment
{
    public uint Id { get; set; }
    public uint TaskId { get; set; }
    public uint UserId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation
    public TaskItem Task { get; set; } = null!;
    public User User { get; set; } = null!;
}
