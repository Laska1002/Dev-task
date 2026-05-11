using DevTaskManager.Application.DTOs.Developer;
using FluentValidation;

namespace DevTaskManager.Application.Validators;

public class CreateDeveloperValidator : AbstractValidator<CreateDeveloperDto>
{
    public CreateDeveloperValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.Cedula)
            .NotEmpty().WithMessage("La cédula es requerida.")
            .Length(10).WithMessage("La cédula debe tener exactamente 10 dígitos.");

        RuleFor(x => x.SeniorityLevel)
            .Must(s => new[] { "junior", "mid", "senior" }.Contains(s.ToLower()))
            .WithMessage("El nivel de seniority debe ser: junior, mid o senior.");

        RuleFor(x => x.AvailabilityStatus)
            .Must(s => new[] { "available", "busy", "vacation" }.Contains(s.ToLower()))
            .WithMessage("El estado de disponibilidad debe ser: available, busy o vacation.");
    }
}

public class UpdateDeveloperValidator : AbstractValidator<UpdateDeveloperDto>
{
    public UpdateDeveloperValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.SeniorityLevel)
            .Must(s => new[] { "junior", "mid", "senior" }.Contains(s.ToLower()))
            .WithMessage("El nivel de seniority debe ser: junior, mid o senior.");

        RuleFor(x => x.AvailabilityStatus)
            .Must(s => new[] { "available", "busy", "vacation" }.Contains(s.ToLower()))
            .WithMessage("El estado de disponibilidad debe ser: available, busy o vacation.");
    }
}
