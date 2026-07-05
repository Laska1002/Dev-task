using DevTaskManager.Domain.Entities;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Application.Strategies;

/// <summary>
/// PATRÓN STRATEGY — Interfaz base para todas las estrategias de transición de estado de tarea.
/// 
/// SOLID - OCP: Para agregar un nuevo estado (ej: Archived), solo se crea una nueva clase que
///              implemente esta interfaz. NO se modifica ninguna clase existente.
/// SOLID - SRP: Cada estrategia tiene una única responsabilidad: manejar una transición específica.
/// </summary>
public interface ITaskStatusTransitionStrategy
{
    /// <summary>Determina si esta estrategia aplica para la transición solicitada.</summary>
    bool CanApply(TaskStatus currentStatus, TaskStatus newStatus);

    /// <summary>Aplica la transición al objeto TaskItem.</summary>
    void Apply(TaskItem task, TaskStatus newStatus);
}
