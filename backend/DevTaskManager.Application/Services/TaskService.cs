using AutoMapper;
using DevTaskManager.Application.DTOs.Task;
using DevTaskManager.Application.Interfaces;
using DevTaskManager.Domain.Entities;
using DevTaskManager.Infrastructure.Repositories;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly TaskRepository _taskRepository;
    private readonly IRepository<TaskComment> _commentRepository;
    private readonly IMapper _mapper;

    public TaskService(TaskRepository taskRepository, IRepository<TaskComment> commentRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<TaskResponseDto> Items, int Total)> GetPagedAsync(
        int page, int size, uint? projectId, string? status, string? priority, uint? assignedTo)
    {
        var (items, total) = await _taskRepository.GetPagedAsync(page, size, projectId, status, priority, assignedTo);
        var dtos = _mapper.Map<IEnumerable<TaskResponseDto>>(items);
        return (dtos, total);
    }

    public async Task<TaskResponseDto?> GetByIdAsync(uint id)
    {
        var task = await _taskRepository.GetByIdWithDetailsAsync(id);
        return task == null ? null : _mapper.Map<TaskResponseDto>(task);
    }

    public async Task<TaskResponseDto> CreateAsync(CreateTaskDto dto, uint createdBy)
    {
        var task = _mapper.Map<TaskItem>(dto);
        
        var year = DateTime.UtcNow.Year;
        var seq = await _taskRepository.GetNextSequenceForYearAsync(year);
        task.TaskCode = $"TSK-{year}-{seq:D3}";
        
        task.CreatedBy = createdBy;
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.AddAsync(task);

        var created = await _taskRepository.GetByIdWithDetailsAsync(task.Id);
        return _mapper.Map<TaskResponseDto>(created);
    }

    public async Task<TaskResponseDto> UpdateAsync(uint id, UpdateTaskDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new Exception("Task no encontrada.");

        _mapper.Map(dto, task);
        task.UpdatedAt = DateTime.UtcNow;

        if (task.Status == TaskStatus.Done && task.CompletedAt == null)
            task.CompletedAt = DateTime.UtcNow;
        else if (task.Status != TaskStatus.Done)
            task.CompletedAt = null;

        await _taskRepository.UpdateAsync(task);

        var updated = await _taskRepository.GetByIdWithDetailsAsync(task.Id);
        return _mapper.Map<TaskResponseDto>(updated);
    }

    public async Task PatchStatusAsync(uint id, string status)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new Exception("Task no encontrada.");

        var newStatus = status == "in_progress" ? TaskStatus.InProgress : Enum.Parse<TaskStatus>(status, true);
        task.Status = newStatus;
        task.UpdatedAt = DateTime.UtcNow;

        if (task.Status == TaskStatus.Done && task.CompletedAt == null)
            task.CompletedAt = DateTime.UtcNow;
        else if (task.Status != TaskStatus.Done)
            task.CompletedAt = null;

        await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteAsync(uint id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task != null)
        {
            await _taskRepository.DeleteAsync(task);
        }
    }

    public async Task<IEnumerable<CommentResponseDto>> GetCommentsAsync(uint taskId)
    {
        var comments = await _taskRepository.GetCommentsAsync(taskId);
        return _mapper.Map<IEnumerable<CommentResponseDto>>(comments);
    }

    public async Task<CommentResponseDto> AddCommentAsync(uint taskId, CreateCommentDto dto, uint userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) throw new Exception("Task no encontrada.");

        var comment = new TaskComment
        {
            TaskId = taskId,
            UserId = userId,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);

        // Fetch again to get the User navigation property loaded for mapping
        var newComment = (await _taskRepository.GetCommentsAsync(taskId)).LastOrDefault(c => c.Id == comment.Id);
        return _mapper.Map<CommentResponseDto>(newComment ?? comment);
    }
}
