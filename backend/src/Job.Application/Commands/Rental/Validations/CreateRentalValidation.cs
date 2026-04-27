namespace Job.Application.Commands.Rental.Validations;

public sealed class CreateRentalValidation : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalValidation()
    {
        RuleFor(x => x.MotoboyIdentifier)
            .NotEmpty()
            .WithMessage("Entregador é obrigatório");

        RuleFor(x => x.MotoIdentifier)
            .NotEmpty()
            .WithMessage("Moto é obrigatória");

        RuleFor(x => x.DateStart)
            .NotEmpty()
            .WithMessage("Data de início é obrigatória");

        RuleFor(x => x.DateEnd)
            .NotEmpty()
            .WithMessage("Data de término é obrigatória")
            .GreaterThan(x => x.DateStart)
            .WithMessage("Data de término deve ser maior que a data de início");

        RuleFor(x => x.DatePreview)
            .NotEmpty()
            .WithMessage("Previsão de término é obrigatória");

        RuleFor(x => x.Plan)
            .IsInEnum()
            .WithMessage("Plano inválido");
    }
}