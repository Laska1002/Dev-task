using System.Text.Json.Serialization;

namespace DevTaskManager.Application.DTOs.Task;

public class CreateTaskDto
{
    public uint ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint? AssignedTo { get; set; }
    public string Status { get; set; } = "todo";
    public string Priority { get; set; } = "medium";
    public decimal? EstimatedHours { get; set; }
    public DateOnly? DueDate { get; set; }
}

public class UpdateTaskDto
{
    // TaskCode NUNCA editable (R2)
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string? TaskCode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint? AssignedTo { get; set; }
    public string Status { get; set; } = "todo";
    public string Priority { get; set; } = "medium";
    public decimal? EstimatedHours { get; set; }
    public DateOnly? DueDate { get; set; }
}

public class PatchTaskStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class CreateCommentDto
{
    public string Comment { get; set; } = string.Empty;
}

public class CommentResponseDto
{
    public uint Id { get; set; }
    public uint TaskId { get; set; }
    public uint UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TaskResponseDto
{
    public uint Id { get; set; }
    public string TaskCode { get; set; } = string.Empty;
    public uint ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public decimal? EstimatedHours { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public uint CreatedBy { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
