using AutoMapper;
using DevTaskManager.Application.DTOs.Task;
using DevTaskManager.Application.Interfaces;
using DevTaskManager.Application.Strategies;
using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Exceptions;
using DevTaskManager.Domain.Interfaces;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IRepository<TaskComment> _commentRepository;
    private readonly IMapper _mapper;
    private readonly TaskStatusTransitionContext _transitionContext;

    public TaskService(
        ITaskRepository taskRepository,
        IRepository<TaskComment> commentRepository,
        IMapper mapper,
        TaskStatusTransitionContext transitionContext)
    {
        _taskRepository = taskRepository;
        _commentRepository = commentRepository;
        _mapper = mapper;
        _transitionContext = transitionContext;
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
        var task = await _taskRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Task", id);

        var previousStatus = task.Status;
        _mapper.Map(dto, task);

        if (task.Status != previousStatus)
            _transitionContext.ApplyTransition(task, task.Status);
        else
            task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);

        var updated = await _taskRepository.GetByIdWithDetailsAsync(task.Id);
        return _mapper.Map<TaskResponseDto>(updated);
    }

    public async Task PatchStatusAsync(uint id, string status)
    {
        var task = await _taskRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Task", id);

        var newStatus = status == "in_progress"
            ? TaskStatus.InProgress
            : Enum.Parse<TaskStatus>(status, true);

        _transitionContext.ApplyTransition(task, newStatus);

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
        var task = await _taskRepository.GetByIdAsync(taskId)
            ?? throw new NotFoundException("Task", taskId);

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
