using AutoMapper;
using DevTaskManager.Application.DTOs.Developer;
using DevTaskManager.Application.DTOs.Project;
using DevTaskManager.Application.DTOs.Task;
using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Enums;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Developer
        CreateMap<Developer, DeveloperResponseDto>()
            .ForMember(d => d.Username, opt => opt.MapFrom(s => s.User != null ? s.User.Username : null))
            .ForMember(d => d.ProjectTypeName, opt => opt.MapFrom(s => s.ProjectType.Name))
            .ForMember(d => d.Technologies, opt => opt.MapFrom(s => s.DeveloperTechnologies.Select(dt => dt.Technology.Name).ToList()))
            .ForMember(d => d.SeniorityLevel, opt => opt.MapFrom(s => s.SeniorityLevel.ToString().ToLower()))
            .ForMember(d => d.AvailabilityStatus, opt => opt.MapFrom(s => s.AvailabilityStatus.ToString().ToLower()));
            
        CreateMap<CreateDeveloperDto, Developer>()
            .ForMember(d => d.SeniorityLevel, opt => opt.MapFrom(s => Enum.Parse<SeniorityLevel>(s.SeniorityLevel, true)))
            .ForMember(d => d.AvailabilityStatus, opt => opt.MapFrom(s => Enum.Parse<AvailabilityStatus>(s.AvailabilityStatus, true)));
            
        CreateMap<UpdateDeveloperDto, Developer>()
            .ForMember(d => d.SeniorityLevel, opt => opt.MapFrom(s => Enum.Parse<SeniorityLevel>(s.SeniorityLevel, true)))
            .ForMember(d => d.AvailabilityStatus, opt => opt.MapFrom(s => Enum.Parse<AvailabilityStatus>(s.AvailabilityStatus, true)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Project
        CreateMap<Project, ProjectResponseDto>()
            .ForMember(d => d.ProjectTypeName, opt => opt.MapFrom(s => s.ProjectType.Name))
            .ForMember(d => d.TechnologyName, opt => opt.MapFrom(s => s.Technology.Name))
            .ForMember(d => d.CreatedByUsername, opt => opt.MapFrom(s => s.Creator.Username))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => 
                s.Status == ProjectStatus.InProgress ? "in_progress" :
                s.Status == ProjectStatus.OnHold ? "on_hold" : 
                s.Status.ToString().ToLower()))
            .ForMember(d => d.Developers, opt => opt.MapFrom(s => s.ProjectDevelopers))
            .ForMember(d => d.TaskCounts, opt => opt.MapFrom(s => new TaskCountsDto
            {
                Todo = s.Tasks.Count(t => t.Status == TaskStatus.Todo),
                InProgress = s.Tasks.Count(t => t.Status == TaskStatus.InProgress),
                Review = s.Tasks.Count(t => t.Status == TaskStatus.Review),
                Done = s.Tasks.Count(t => t.Status == TaskStatus.Done),
                Blocked = s.Tasks.Count(t => t.Status == TaskStatus.Blocked)
            }));

        CreateMap<ProjectDeveloper, ProjectDeveloperDto>()
            .ForMember(d => d.DeveloperId, opt => opt.MapFrom(s => s.DeveloperId))
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Developer.FullName))
            .ForMember(d => d.RoleInProject, opt => opt.MapFrom(s => s.RoleInProject))
            .ForMember(d => d.SeniorityLevel, opt => opt.MapFrom(s => s.Developer.SeniorityLevel.ToString().ToLower()))
            .ForMember(d => d.ProjectTypeName, opt => opt.MapFrom(s => s.Developer.ProjectType.Name));

        CreateMap<CreateProjectDto, Project>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => 
                s.Status == "in_progress" ? ProjectStatus.InProgress :
                s.Status == "on_hold" ? ProjectStatus.OnHold :
                Enum.Parse<ProjectStatus>(s.Status, true)));

        CreateMap<UpdateProjectDto, Project>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => 
                s.Status == "in_progress" ? ProjectStatus.InProgress :
                s.Status == "on_hold" ? ProjectStatus.OnHold :
                Enum.Parse<ProjectStatus>(s.Status, true)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // TaskItem
        CreateMap<TaskItem, TaskResponseDto>()
            .ForMember(d => d.ProjectName, opt => opt.MapFrom(s => s.Project.Name))
            .ForMember(d => d.AssignedToName, opt => opt.MapFrom(s => s.AssignedDeveloper != null ? s.AssignedDeveloper.FullName : null))
            .ForMember(d => d.CreatedByUsername, opt => opt.MapFrom(s => s.Creator.Username))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => 
                s.Status == TaskStatus.InProgress ? "in_progress" : s.Status.ToString().ToLower()))
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString().ToLower()));

        CreateMap<CreateTaskDto, TaskItem>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => 
                s.Status == "in_progress" ? TaskStatus.InProgress : Enum.Parse<TaskStatus>(s.Status, true)))
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => Enum.Parse<TaskPriority>(s.Priority, true)));

        CreateMap<UpdateTaskDto, TaskItem>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => 
                s.Status == "in_progress" ? TaskStatus.InProgress : Enum.Parse<TaskStatus>(s.Status, true)))
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => Enum.Parse<TaskPriority>(s.Priority, true)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<TaskComment, CommentResponseDto>()
            .ForMember(d => d.Username, opt => opt.MapFrom(s => s.User.Username));
    }
}
