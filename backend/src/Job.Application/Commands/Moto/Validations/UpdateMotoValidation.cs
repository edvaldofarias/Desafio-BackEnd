namespace Job.Application.Commands.Moto.Validations;

public sealed class UpdateMotoValidation : AbstractValidator<UpdateMotoCommand>
{
    public UpdateMotoValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id é obrigatório");

        RuleFor(x => x.Plate)
            .NotEmpty()
            .WithMessage("Placa é obrigatória")
            .Matches("[a-zA-Z]{3}[0-9]{1}[a-zA-Z]{1}[0-9]{2}|[a-zA-Z]{3}[0-9]{4}")
            .WithMessage("Placa inválida");
    }
}