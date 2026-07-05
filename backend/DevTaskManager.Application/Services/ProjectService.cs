using AutoMapper;
using DevTaskManager.Application.DTOs.Project;
using DevTaskManager.Application.Interfaces;
using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Exceptions;
using DevTaskManager.Domain.Interfaces;

namespace DevTaskManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IRepository<ProjectDeveloper> _projectDeveloperRepository;
    private readonly IMapper _mapper;

    public ProjectService(
        IProjectRepository projectRepository,
        IRepository<ProjectDeveloper> projectDeveloperRepository,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _projectDeveloperRepository = projectDeveloperRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ProjectResponseDto> Items, int Total)> GetPagedAsync(
        int page, int size, string? status, uint? typeId)
    {
        var (items, total) = await _projectRepository.GetPagedAsync(page, size, status, typeId);
        var dtos = _mapper.Map<IEnumerable<ProjectResponseDto>>(items);
        return (dtos, total);
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(uint id)
    {
        var project = await _projectRepository.GetByIdWithDetailsAsync(id);
        return project == null ? null : _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto, uint createdBy)
    {
        var project = _mapper.Map<Project>(dto);
        
        var year = DateTime.UtcNow.Year;
        var seq = await _projectRepository.GetNextSequenceForYearAsync(year);
        project.ProjectCode = $"PRJ-{year}-{seq:D3}";
        
        project.CreatedBy = createdBy;
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.AddAsync(project);

        var created = await _projectRepository.GetByIdWithDetailsAsync(project.Id);
        return _mapper.Map<ProjectResponseDto>(created);
    }

    public async Task<ProjectResponseDto> UpdateAsync(uint id, UpdateProjectDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Proyecto", id);

        _mapper.Map(dto, project);
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);

        var updated = await _projectRepository.GetByIdWithDetailsAsync(project.Id);
        return _mapper.Map<ProjectResponseDto>(updated);
    }

    public async Task DeleteAsync(uint id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project != null)
        {
            await _projectRepository.DeleteAsync(project);
        }
    }

    public async Task AssignDevelopersAsync(uint projectId, AssignDevelopersDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(projectId)
            ?? throw new NotFoundException("Proyecto", projectId);

        foreach (var devId in dto.DeveloperIds)
        {
            var existing = await _projectDeveloperRepository.FindAsync(pd => pd.ProjectId == projectId && pd.DeveloperId == devId);
            if (!existing.Any())
            {
                await _projectDeveloperRepository.AddAsync(new ProjectDeveloper
                {
                    ProjectId = projectId,
                    DeveloperId = devId,
                    RoleInProject = dto.RoleInProject,
                    AssignedAt = DateTime.UtcNow
                });
            }
        }
    }

    public async Task RemoveDeveloperAsync(uint projectId, uint developerId)
    {
        var relations = await _projectDeveloperRepository.FindAsync(pd => pd.ProjectId == projectId && pd.DeveloperId == developerId);
        var relation = relations.FirstOrDefault();
        if (relation != null)
        {
            await _projectDeveloperRepository.DeleteAsync(relation);
        }
    }

    public async Task<IEnumerable<ProjectDeveloperDto>> GetDevelopersAsync(uint projectId)
    {
        var project = await _projectRepository.GetByIdWithDetailsAsync(projectId);
        if (project == null) return new List<ProjectDeveloperDto>();
        
        return _mapper.Map<IEnumerable<ProjectDeveloperDto>>(project.ProjectDevelopers);
    }
}
