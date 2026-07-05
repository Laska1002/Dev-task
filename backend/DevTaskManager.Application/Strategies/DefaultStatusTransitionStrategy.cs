using DevTaskManager.Domain.Entities;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Application.Strategies;

/// <summary>
/// PATRÓN STRATEGY — Estrategia concreta: transición a cualquier estado que NO sea Done.
/// 
/// SOLID - SRP: Maneja todas las transiciones "normales" (todo, in_progress, review, blocked).
/// Cuando se mueve a un estado distinto de Done, limpia la fecha de completado.
/// </summary>
public class DefaultStatusTransitionStrategy : ITaskStatusTransitionStrategy
{
    public bool CanApply(TaskStatus currentStatus, TaskStatus newStatus)
        => newStatus != TaskStatus.Done;

    public void Apply(TaskItem task, TaskStatus newStatus)
    {
        task.Status = newStatus;
        // Si la tarea deja de estar Done, se limpia la fecha de completado
        task.CompletedAt = null;
        task.UpdatedAt = DateTime.UtcNow;
    }
}
