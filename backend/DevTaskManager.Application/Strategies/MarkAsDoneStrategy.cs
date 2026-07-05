using DevTaskManager.Domain.Entities;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Application.Strategies;

/// <summary>
/// PATRÓN STRATEGY — Estrategia concreta: transición al estado "Done".
/// 
/// SOLID - SRP: Solo se encarga de manejar la transición hacia el estado Done.
/// Cuando se marca como Done, registra automáticamente la fecha y hora de completado.
/// </summary>
public class MarkAsDoneStrategy : ITaskStatusTransitionStrategy
{
    public bool CanApply(TaskStatus currentStatus, TaskStatus newStatus)
        => newStatus == TaskStatus.Done;

    public void Apply(TaskItem task, TaskStatus newStatus)
    {
        task.Status = newStatus;
        // Al completar la tarea, se registra la fecha de completado
        if (task.CompletedAt == null)
            task.CompletedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
    }
}
