using DevTaskManager.Application.DTOs.Task;

namespace DevTaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<(IEnumerable<TaskResponseDto> Items, int Total)> GetPagedAsync(
        int page, int size, uint? projectId, string? status, string? priority, uint? assignedTo);
    Task<TaskResponseDto?> GetByIdAsync(uint id);
    Task<TaskResponseDto> CreateAsync(CreateTaskDto dto, uint createdBy);
    Task<TaskResponseDto> UpdateAsync(uint id, UpdateTaskDto dto);
    Task PatchStatusAsync(uint id, string status);
    Task DeleteAsync(uint id);
    Task<IEnumerable<CommentResponseDto>> GetCommentsAsync(uint taskId);
    Task<CommentResponseDto> AddCommentAsync(uint taskId, CreateCommentDto dto, uint userId);
}
