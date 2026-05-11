using DevTaskManager.Application.DTOs.Project;
using DevTaskManager.Domain.Entities;

namespace DevTaskManager.Application.Interfaces;

public interface IProjectService
{
    Task<(IEnumerable<ProjectResponseDto> Items, int Total)> GetPagedAsync(
        int page, int size, string? status, uint? typeId);
    Task<ProjectResponseDto?> GetByIdAsync(uint id);
    Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto, uint createdBy);
    Task<ProjectResponseDto> UpdateAsync(uint id, UpdateProjectDto dto);
    Task DeleteAsync(uint id);
    Task AssignDevelopersAsync(uint projectId, AssignDevelopersDto dto);
    Task RemoveDeveloperAsync(uint projectId, uint developerId);
    Task<IEnumerable<ProjectDeveloperDto>> GetDevelopersAsync(uint projectId);
}
