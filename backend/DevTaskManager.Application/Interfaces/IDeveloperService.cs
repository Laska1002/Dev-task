using DevTaskManager.Application.DTOs.Developer;

namespace DevTaskManager.Application.Interfaces;

public interface IDeveloperService
{
    Task<(IEnumerable<DeveloperResponseDto> Items, int Total)> GetPagedAsync(
        int page, int size, string? status, string? seniority, uint? technologyId = null, uint? projectTypeId = null);
    Task<DeveloperResponseDto?> GetByIdAsync(uint id);
    Task<DeveloperResponseDto> CreateAsync(CreateDeveloperDto dto);
    Task<DeveloperResponseDto> UpdateAsync(uint id, UpdateDeveloperDto dto);
    Task SoftDeleteAsync(uint id);
}
