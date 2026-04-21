namespace Job.Application.Commands.Rental.Validations;

public sealed class CancelRentalValidation : AbstractValidator<CancelRentalCommand>
{
    public CancelRentalValidation()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Identificador é obrigatório");

        RuleFor(x => x.DateReturn)
            .NotEmpty()
            .WithMessage("Data de devolução é obrigatória");
    }
}