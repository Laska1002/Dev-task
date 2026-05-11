using DevTaskManager.Application.DTOs.Task;
using FluentValidation;

namespace DevTaskManager.Application.Validators;

public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título de la tarea es requerido.")
            .MaximumLength(200);

        RuleFor(x => x.ProjectId)
            .GreaterThan(0u).WithMessage("El proyecto es requerido.");

        RuleFor(x => x.Status)
            .Must(s => new[] { "todo", "in_progress", "review", "done", "blocked" }
                .Contains(s.ToLower()))
            .WithMessage("Estado inválido.");

        RuleFor(x => x.Priority)
            .Must(p => new[] { "low", "medium", "high", "critical" }.Contains(p.ToLower()))
            .WithMessage("Prioridad inválida.");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0m).When(x => x.EstimatedHours.HasValue)
            .WithMessage("Las horas estimadas deben ser positivas.");
    }
}

public class UpdateTaskValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título de la tarea es requerido.")
            .MaximumLength(200);

        RuleFor(x => x.Status)
            .Must(s => new[] { "todo", "in_progress", "review", "done", "blocked" }
                .Contains(s.ToLower()))
            .WithMessage("Estado inválido.");

        RuleFor(x => x.Priority)
            .Must(p => new[] { "low", "medium", "high", "critical" }.Contains(p.ToLower()))
            .WithMessage("Prioridad inválida.");
    }
}
