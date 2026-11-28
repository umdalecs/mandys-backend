using FluentValidation;
using Mandys.Application.DTOs;

namespace Mandys.Api.Validators;

public class CreateDrawerReconciliationRequestValidator : AbstractValidator<CreateDrawerReconciliationRequest>
{
    public CreateDrawerReconciliationRequestValidator()
    {
        RuleFor(x => x.CountedAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La cantidad contada no puede ser negativa.")
            .Must(a => Math.Round(a, 2) == a)
            .WithMessage("La cantidad contada no puede tener más de 2 decimales.");
    }
}

public class ReconcileDrawerReasonRequestValidator : AbstractValidator<ReconcileDrawerReasonRequest>
{
    public ReconcileDrawerReasonRequestValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("El motivo de la conciliación es obligatorio.")
            .MaximumLength(500)
            .WithMessage("El motivo de la conciliación no puede superar los 500 caracteres.");
    }
}
