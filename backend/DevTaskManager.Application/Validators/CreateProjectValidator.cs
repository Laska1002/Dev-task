using DevTaskManager.Application.DTOs.Project;
using FluentValidation;

namespace DevTaskManager.Application.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del proyecto es requerido.")
            .MaximumLength(150);

        RuleFor(x => x.ProjectTypeId)
            .GreaterThan(0u).WithMessage("El tipo de proyecto es requerido.");

        RuleFor(x => x.Status)
            .Must(s => new[] { "planning", "in_progress", "on_hold", "completed", "cancelled" }
                .Contains(s.ToLower()))
            .WithMessage("Estado inválido.");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0m).When(x => x.EstimatedHours.HasValue)
            .WithMessage("Las horas estimadas deben ser positivas.");
    }
}

public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del proyecto es requerido.")
            .MaximumLength(150);

        RuleFor(x => x.ProjectTypeId)
            .GreaterThan(0u).WithMessage("El tipo de proyecto es requerido.");

        RuleFor(x => x.Status)
            .Must(s => new[] { "planning", "in_progress", "on_hold", "completed", "cancelled" }
                .Contains(s.ToLower()))
            .WithMessage("Estado inválido.");
    }
}
