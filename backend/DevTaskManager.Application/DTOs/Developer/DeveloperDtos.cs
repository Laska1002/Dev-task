using System.Text.Json.Serialization;

namespace DevTaskManager.Application.DTOs.Developer;

public class CreateDeveloperDto
{
    public uint? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public uint ProjectTypeId { get; set; }
    public List<uint> TechnologyIds { get; set; } = new();
    public string SeniorityLevel { get; set; } = "mid";
    public string AvailabilityStatus { get; set; } = "available";
}

public class UpdateDeveloperDto
{
    public uint? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    // Cédula NO editable si ya tiene valor — el service lo ignorará
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string? Cedula { get; set; }
    public uint ProjectTypeId { get; set; }
    public List<uint> TechnologyIds { get; set; } = new();
    public string SeniorityLevel { get; set; } = "mid";
    public string AvailabilityStatus { get; set; } = "available";
}

public class DeveloperResponseDto
{
    public uint Id { get; set; }
    public uint? UserId { get; set; }
    public string? Username { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public uint ProjectTypeId { get; set; }
    public string ProjectTypeName { get; set; } = string.Empty;
    public List<string> Technologies { get; set; } = new();
    public string SeniorityLevel { get; set; } = string.Empty;
    public string AvailabilityStatus { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
