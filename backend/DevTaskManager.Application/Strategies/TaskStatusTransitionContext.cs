using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Exceptions;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Application.Strategies;

/// <summary>
/// PATRÓN STRATEGY — Contexto que selecciona y ejecuta la estrategia correcta.
/// 
/// Esta clase actúa como el "contexto" del patrón Strategy: recibe todas las estrategias
/// registradas por inyección de dependencias y delega la ejecución a la que corresponda.
/// 
/// SOLID - OCP: Para agregar un nuevo tipo de transición, solo se registra una nueva
///              implementación de ITaskStatusTransitionStrategy en Program.cs.
///              Este contexto NO necesita ser modificado.
/// </summary>
public class TaskStatusTransitionContext
{
    private readonly IEnumerable<ITaskStatusTransitionStrategy> _strategies;

    public TaskStatusTransitionContext(IEnumerable<ITaskStatusTransitionStrategy> strategies)
    {
        _strategies = strategies;
    }

    /// <summary>
    /// Selecciona la estrategia apropiada y aplica la transición de estado.
    /// </summary>
    /// <exception cref="BusinessValidationException">Si no existe estrategia para la transición.</exception>
    public void ApplyTransition(TaskItem task, TaskStatus newStatus)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CanApply(task.Status, newStatus))
            ?? throw new BusinessValidationException(
                $"No se puede realizar la transición de '{task.Status}' a '{newStatus}'.");

        strategy.Apply(task, newStatus);
    }
}
