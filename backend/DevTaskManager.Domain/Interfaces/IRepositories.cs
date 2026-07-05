using System.Linq.Expressions;
using DevTaskManager.Domain.Entities;

namespace DevTaskManager.Domain.Interfaces;

/// <summary>
/// Contrato genérico de repositorio.
/// Ubicado en Domain para que Infrastructure pueda implementarlo sin dependencias circulares.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(uint id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

/// <summary>
/// PATRÓN REPOSITORY — Contrato para el repositorio de TaskItem.
/// SOLID - DIP: Los servicios dependen de esta interfaz, no de la clase concreta TaskRepository.
/// </summary>
public interface ITaskRepository : IRepository<TaskItem>
{
    Task<(IEnumerable<TaskItem> Items, int Total)> GetPagedAsync(
        int page, int size, uint? projectId, string? status, string? priority, uint? assignedTo);
    Task<TaskItem?> GetByIdWithDetailsAsync(uint id);
    Task<IEnumerable<TaskComment>> GetCommentsAsync(uint taskId);
    Task<int> GetNextSequenceForYearAsync(int year);
}

/// <summary>
/// PATRÓN REPOSITORY — Contrato para el repositorio de Developer.
/// SOLID - DIP: DeveloperService depende de esta interfaz.
/// </summary>
public interface IDeveloperRepository : IRepository<Developer>
{
    Task<(IEnumerable<Developer> Items, int Total)> GetPagedAsync(
        int page, int size, string? status, string? seniority,
        uint? technologyId = null, uint? projectTypeId = null);
    Task<Developer?> GetByIdWithUserAsync(uint id);
    Task<bool> ExistsByCedulaAsync(string cedula, uint? excludeId = null);
    Task<int> GetNextSequenceAsync();
}

/// <summary>
/// PATRÓN REPOSITORY — Contrato para el repositorio de Project.
/// SOLID - DIP: ProjectService depende de esta interfaz.
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    Task<(IEnumerable<Project> Items, int Total)> GetPagedAsync(
        int page, int size, string? status, uint? typeId);
    Task<Project?> GetByIdWithDetailsAsync(uint id);
    Task<IEnumerable<Developer>> GetDevelopersAsync(uint projectId);
    Task<int> GetNextSequenceForYearAsync(int year);
}
