using System.Text.Json.Serialization;

namespace DevTaskManager.Application.DTOs.Project;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint ProjectTypeId { get; set; }
    public uint TechnologyId { get; set; }
    public string Status { get; set; } = "planning";
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
}

public class UpdateProjectDto
{
    // ProjectCode NUNCA editable — ignorado del body (R2)
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string? ProjectCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint ProjectTypeId { get; set; }
    public uint TechnologyId { get; set; }
    public string Status { get; set; } = "planning";
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
}

public class AssignDevelopersDto
{
    public List<uint> DeveloperIds { get; set; } = new();
    public string? RoleInProject { get; set; }
}

public class ProjectDeveloperDto
{
    public uint DeveloperId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? RoleInProject { get; set; }
    public string SeniorityLevel { get; set; } = string.Empty;
    public string ProjectTypeName { get; set; } = string.Empty;
}

public class TaskCountsDto
{
    public int Todo { get; set; }
    public int InProgress { get; set; }
    public int Review { get; set; }
    public int Done { get; set; }
    public int Blocked { get; set; }
}

public class ProjectResponseDto
{
    public uint Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint ProjectTypeId { get; set; }
    public string ProjectTypeName { get; set; } = string.Empty;
    public uint TechnologyId { get; set; }
    public string TechnologyName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public uint CreatedBy { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ProjectDeveloperDto> Developers { get; set; } = new();
    public TaskCountsDto? TaskCounts { get; set; }
}
